using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Extensions;
using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.Models.Entities;
using Plat4Me.DialRuleEngine.Application.Repositories;
using System.Text.Json;

namespace Plat4Me.DialRuleEngine.Infrastructure.Repositories;

public class LeadRepository : ILeadRepository
{
    private readonly DialDbContext _context;
    private readonly ILogger<LeadRepository> _logger;

    private IQueryable<Lead> ProcessingAvailableLeads() => _context.Leads
        .AsNoTracking()
        .Where(r => r.DeletedOn == null
                    && r.DuplicateOf == null
                    && (r.FreezeTo == null || r.FreezeTo < DateTimeOffset.UtcNow)
                    && !r.LeadBlacklist.Any());

    public LeadRepository(
        DialDbContext context,
        ILogger<LeadRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Client>> GetAllClients(
        CancellationToken ct = default)
    {
        var entities = await _context.Clients
            .ToArrayAsync(ct);

        return entities;
    }

    public async Task<IReadOnlyCollection<TrackedLead>> GetForPreProcessing(
        long clientId,
        CancellationToken ct = default)
    {
        var q = ProcessingAvailableLeads()
            .Where(r => r.ClientId == clientId
                        && (!r.SystemStatus.HasValue
                            || !LeadSystemStatusTypesExtension.Unavailable
                                .Contains(r.SystemStatus!.Value)));

        var results = await GetForProcessing(q, null)
            .ToArrayAsync(ct);
        
        return results;
    }

    public async Task<IReadOnlyCollection<TrackedLead>> GetForPostProcessing(
        long clientId,
        IEnumerable<long>? leadIds = null,
        CancellationToken ct = default)
    {
        var q = ProcessingAvailableLeads()
            .Include(r => r.LeadHistory)
            .Where(r => r.ClientId == clientId
                        && r.SystemStatus == LeadSystemStatusTypes.PostProcessing);

        var results = await GetForProcessing(q, leadIds)
            .ToArrayAsync(ct);

        return results;
    }

    public async Task<IReadOnlyCollection<TrackedLead>> GetForImportedProcessing(
        long clientId,
        IEnumerable<long>? leadIds = null,
        CancellationToken ct = default)
    {
        var q = ProcessingAvailableLeads()
            .Where(r => r.ClientId == clientId
                        && r.SystemStatus == LeadSystemStatusTypes.Imported);

        var results = await GetForProcessing(q, leadIds)
            .ToArrayAsync(ct);

        return results;
    }


    private static IQueryable<TrackedLead> GetForProcessing(
        IQueryable<Lead> q,
        IEnumerable<long>? leadIds)
    {
        if (leadIds is not null)
            q = q.Where(r => leadIds.Contains(r.Id));

        var results = q.Select(r => ToTrackedLead(r));

        return results;
    }

    private static TrackedLead ToTrackedLead(Lead lead)
    {
        return new TrackedLead(
                lead.Id,
                lead.FirstName,
                lead.LastName,
                lead.Phone,
                lead.Status,
                lead.SystemStatus,
                lead.RegistrationTime,
                lead.RemindOn,
                lead.LastCallAgentId,
                lead.CountryCode,
                lead.DeletedOn,
                lead.Timezone,
                lead.IsTest,
                lead.AssignedAgentId,
                lead.FreezeTo,
                lead.LeadHistory
                    .Where(x => x.ActionType == LeadHistoryActionType.Status)
                    .Select(x => new KeyValuePair<DateTimeOffset, string>(x.CreatedAt, x.Changes)),
                lead.MetaData
            );
    }

    public async Task ClearSystemStatuses(
        IEnumerable<long> leadIds,
        CancellationToken ct = default)
    {
        var entities = await _context.Leads
            .Where(r => leadIds.Contains(r.Id))
            .ToArrayAsync(ct);

        var now = DateTimeOffset.UtcNow;

        foreach (var item in entities)
        {
            await AddLeadHistories(item.Id, LeadHistoryActionType.System, createdBy: null, ct,
                new(nameof(Lead.SystemStatus), item.SystemStatus, null),
                new(nameof(Lead.LastUpdateTime), item.LastUpdateTime, now));

            item.SystemStatus = null;
            item.LastUpdateTime = now;
        }

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Lead Ids {leadIds} clear system status",
            string.Join(", ", leadIds));
    }

    public async Task<Lead?> UpdateSystemStatus(
        long leadId,
        LeadSystemStatusTypes? systemStatus,
        CancellationToken ct = default)
    {
        var entity = await _context.Leads
            .Where(r => r.Id == leadId)
            .FirstOrDefaultAsync(ct);

        if (entity is null) return null;

        var now = DateTimeOffset.UtcNow;

        await AddLeadHistories(entity.Id, LeadHistoryActionType.System, createdBy: null, ct,
            new(nameof(Lead.SystemStatus), entity.SystemStatus, systemStatus),
            new(nameof(Lead.LastUpdateTime), entity.LastUpdateTime, now));

        entity.SystemStatus = systemStatus;
        entity.LastUpdateTime = now;

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Lead Id {leadId} update system status {systemStatus}",
            leadId, systemStatus);

        return entity;
    }

    public async Task UpdateLeads(
        IEnumerable<TrackedLead> leads,
        CancellationToken ct = default)
    {
        foreach (var lead in leads)
        {
            var changes = lead.GetTrackedChanges();
            if (!changes.Any()) continue;

            var entity = await _context.Leads
                .Where(r => r.Id == lead.LeadId)
                .FirstOrDefaultAsync(ct);

            if (entity is null)
                throw new ArgumentNullException(nameof(lead.LeadId), "Lead not found");

            entity.FirstName = lead.FirstName;
            entity.LastName = lead.LastName;
            entity.Phone = lead.LeadPhone;
            entity.RegistrationTime = lead.RegistrationTime;
            entity.CountryCode = lead.CountryCode;
            entity.RemindOn = lead.RemindOn;
            entity.LastCallAgentId = lead.LastCallAgentId;
            entity.AssignedAgentId = lead.AssignedAgentId;
            entity.DeletedOn = lead.DeletedOn;
            entity.FreezeTo = lead.FreezeTo;
            entity.Status = lead.Status;
            entity.LastUpdateTime = DateTimeOffset.UtcNow;

            await _context.LeadHistory.AddRangeAsync(changes, ct);
        }

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Lead Ids {leadIds} update",
            string.Join(", ", leads.Select(r => r.LeadId)));
    }

    public async Task UpdateFirstTimeQueued(
        IEnumerable<long> leadIds,
        CancellationToken ct = default)
    {
        var leads = await _context.Leads
            .Where(x => !x.FirstTimeQueuedOn.HasValue
                        && leadIds.Contains(x.Id))
            .ToArrayAsync(ct);

        var now = DateTimeOffset.UtcNow;

        foreach (var lead in leads)
        {
            await AddLeadHistories(lead.Id, LeadHistoryActionType.Data, createdBy: null, ct,
                new ValueChanges<object?>(nameof(Lead.FirstTimeQueuedOn), lead.FirstTimeQueuedOn, now));

            lead.FirstTimeQueuedOn = now;
        }

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Set first time queued for lead Ids {leadIds}",
            string.Join(", ", leadIds));
    }

    public async Task ResetAssignation(
        IEnumerable<long> agentIds,
        CancellationToken ct = default)
    {
        var leads = await _context.Leads
            .Where(r => r.AssignedAgentId.HasValue
                        && agentIds.Contains(r.AssignedAgentId.Value))
            .ToArrayAsync(ct);

        foreach (var lead in leads)
        {
            await AddLeadHistories(lead.Id, LeadHistoryActionType.Data, createdBy: null, ct,
                new ValueChanges<object?>(nameof(Lead.AssignedAgentId), lead.AssignedAgentId, null));

            lead.AssignedAgentId = null;
        }

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Reset assignation for lead Ids {leadIds}",
            string.Join(", ", leads.Select(r => r.Id)));
    }

    public async Task AddLeadHistories(
        long leadId,
        LeadHistoryActionType actionType,
        long? createdBy = null,
        CancellationToken ct = default,
        params ValueChanges<object?>[] changes)
    {
        var changesDto = new LeadHistoryChangesDto<object?>
        {
            Properties = changes.ToList()
        };

        var historyEntry = new LeadHistory
        {
            LeadId = leadId,
            ActionType = actionType,
            Changes = JsonSerializer.Serialize(changesDto, JsonSettingsExtensions.Default),
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = createdBy,
        };

        await _context.LeadHistory.AddAsync(historyEntry, ct);
    }

    public async Task<IReadOnlyCollection<LeadStatusDto>> GetLeadsWithSystemStatus(CancellationToken ct = default)
    {
        return await _context.Leads
            .Where(x => x.SystemStatus != null && x.DuplicateOfId == null && x.DeletedOn == null)
            .Select(x => new LeadStatusDto(
                x.Id,
                x.ClientId,
                x.Status,
                x.RemindOn,
                x.SystemStatus
            ))
            .ToArrayAsync(ct);
    }
}
