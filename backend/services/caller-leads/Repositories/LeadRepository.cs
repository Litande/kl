using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Plat4Me.DialLeadCaller.Application.Enums;
using Plat4Me.DialLeadCaller.Application.Extensions;
using Plat4Me.DialLeadCaller.Application.Models;
using Plat4Me.DialLeadCaller.Application.Models.Entities;
using Plat4Me.DialLeadCaller.Application.Repositories;
using System.Text.Json;

namespace Plat4Me.DialLeadCaller.Infrastructure.Repositories;

public class LeadRepository : ILeadRepository
{
    private readonly DialDbContext _context;
    private readonly ILogger<LeadRepository> _logger;

    public LeadRepository(
        DialDbContext context,
        ILogger<LeadRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Lead?> UpdateStatusAndGet(
        long clientId,
        long leadId,
        LeadSystemStatusTypes? systemStatus,
        LeadStatusTypes? status = null,
        CancellationToken ct = default)
    {
        var entity = await _context.Leads
            .Where(r => r.ClientId == clientId
                        && r.Id == leadId)
            .FirstOrDefaultAsync(ct);

        if (entity is null) return entity;

        var now = DateTimeOffset.UtcNow;

        await AddLeadHistories(entity.Id, LeadHistoryActionType.System, null, ct,
            new(nameof(Lead.SystemStatus), entity.SystemStatus, systemStatus),
            new(nameof(Lead.LastUpdateTime), entity.LastUpdateTime, now));

        if (status.HasValue)
        {
            await AddLeadHistories(entity.Id, LeadHistoryActionType.Status, null, ct,
                new ValueChanges<object?>(nameof(Lead.Status), entity.Status, status));

            entity.Status = status.Value;
        }

        entity.SystemStatus = systemStatus;
        entity.LastUpdateTime = now;

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Lead Id {leadId} update system status {systemStatus} and status {status}",
            leadId, systemStatus, status);

        return entity;
    }

    public async Task UpdateSystemStatuses(
        long clientId,
        IEnumerable<long> leadIds,
        LeadSystemStatusTypes? systemStatus,
        CancellationToken ct = default)
    {
        var entities = await _context.Leads
            .Where(r => r.ClientId == clientId
                        && leadIds.Contains(r.Id))
            .ToArrayAsync(ct);
        var now = DateTimeOffset.UtcNow;

        foreach (var item in entities)
        {
            await AddLeadHistories(item.Id, LeadHistoryActionType.System, null, ct,
                new(nameof(Lead.SystemStatus), item.SystemStatus, systemStatus),
                new(nameof(Lead.LastUpdateTime), item.LastUpdateTime, now));

            item.SystemStatus = systemStatus;
            item.LastUpdateTime = now;
        }

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Lead Ids {leadIds} update system status {systemStatus}",
            string.Join(", ", leadIds), systemStatus);
    }

    public async Task<Lead?> GetLeadById(long clientId, long leadId, CancellationToken ct = default)
    {
        return await _context.Leads.AsNoTracking()
            .Where(r => r.ClientId == clientId
                        && r.Id == leadId)
            .FirstOrDefaultAsync(ct);
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
    public async Task<Lead?> SaveFeedbackAndGet(
       long clientId,
       long agentId,
       bool isGenerated,
       long leadId,
       LeadSystemStatusTypes? systemStatus,
       LeadStatusTypes status,
       DateTimeOffset? remindOn = null,
       CancellationToken ct = default)
    {
        var lead = await _context.Leads
            .Where(r => r.ClientId == clientId
                        && r.Id == leadId)
            .FirstOrDefaultAsync(ct);

        if (lead is null) return null;

        var now = DateTimeOffset.UtcNow;
        var createdBy = isGenerated ? (long?)null : agentId;

        await AddLeadHistories(lead.Id, LeadHistoryActionType.System, createdBy, ct,
            new(nameof(Lead.SystemStatus), lead.SystemStatus, systemStatus),
            new(nameof(Lead.LastUpdateTime), lead.LastUpdateTime, now),
            new(nameof(Lead.AssignedAgentId), lead.AssignedAgentId, null));

        await AddLeadHistories(lead.Id, LeadHistoryActionType.Status, createdBy, ct,
            new ValueChanges<object?>(nameof(Lead.Status), lead.Status, status));

        var changes = new List<ValueChanges<object?>>
        {
            new(nameof(Lead.LastCallAgentId), lead.LastCallAgentId, agentId),
        };

        if (remindOn.HasValue)
            changes.Add(new(nameof(Lead.RemindOn), lead.RemindOn, remindOn));

        await AddLeadHistories(lead.Id, LeadHistoryActionType.Data, createdBy, ct, changes.ToArray());

        lead.LastCallAgentId = agentId;
        lead.SystemStatus = systemStatus;
        lead.RemindOn = remindOn;
        lead.Status = status;
        lead.LastUpdateTime = now;
        lead.AssignedAgentId = null;

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Set feedback for lead Id {leadId}; agent Id {agentId}; system status {systemStatus}; remind on {remindOn}; status {status}",
            leadId, agentId, systemStatus, remindOn, status);

        return lead;
    }
}
