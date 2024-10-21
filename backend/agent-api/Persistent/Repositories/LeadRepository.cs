using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Plat4Me.DialAgentApi.Application.Common;
using Plat4Me.DialAgentApi.Application.Enums;
using Plat4Me.DialAgentApi.Application.Models;
using Plat4Me.DialAgentApi.Application.Models.Requests;
using Plat4Me.DialAgentApi.Application.Models.Responses;
using Plat4Me.DialAgentApi.Persistent.Entities;
using Plat4Me.DialAgentApi.Persistent.Entities.Projections;
using Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialAgentApi.Persistent.Repositories;

public class LeadRepository : RepositoryBase, ILeadRepository
{
    private readonly DialDbContext _context;
    private readonly ILeadCacheRepository _leadCacheRepository;
    private readonly ILogger<LeadRepository> _logger;

    public LeadRepository(
        DialDbContext context,
        ILogger<LeadRepository> logger,
        ILeadCacheRepository leadCacheRepository)
    {
        _context = context;
        _logger = logger;
        _leadCacheRepository = leadCacheRepository;
    }

    public async Task<Lead?> GetById(
        long clientId,
        long leadId,
        CancellationToken ct = default)
    {
        var entity = await _context.Leads
            .AsNoTracking()
            .Where(i => i.ClientId == clientId
                        && i.Id == leadId)
            .FirstOrDefaultAsync(ct);

        return entity;
    }

    public async Task<Lead?> GetWithDataSource(
        long clientId,
        long leadId,
        CancellationToken ct = default)
    {
        var entity = await _context.Leads
            .AsNoTracking()
            .Include(x => x.DataSource)
            .Where(i => i.ClientId == clientId
                        && i.Id == leadId)
            .FirstOrDefaultAsync(ct);

        return entity;
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

    public async Task<PaginatedResponse<FutureCallBackProjection>> GetAllFeatureCallBacks(
        long clientId,
        long agentId,
        PaginationRequest pagination,
        CancellationToken ct = default)
    {
        var leads = await _context.Leads
            .Where(x => x.ClientId == clientId
                        && x.AssignedAgentId == agentId
                        && x.RemindOn.HasValue
                        && x.RemindOn.Value.Date <= DateTimeOffset.UtcNow.Date)
            .Select(x => new FutureCallBackProjection
            {
                LeadId = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                CountryCode = x.CountryCode,
                Campaign = x.DataSource.Name,
                Email = x.Email,
                NextCallAt = x.RemindOn,
                LeadStatus = x.Status,
                Phone = x.Phone,
                RegisteredAt = x.RegistrationTime,
                LastCallAt = GetLastCall(x.CallDetailRecords),
                TotalCallsSecond = x.CallDetailRecords.Sum(y => y.CallDuration) ?? 0
            }).ToDictionaryAsync(x => x.LeadId, x => x, ct);

        await FillLeadsWeights(leads);

        return CreatePaginatedResponse(leads.Values, pagination);
    }

    private async Task FillLeadsWeights(IDictionary<long, FutureCallBackProjection> collection)
    {
        var cachedLeads = await _leadCacheRepository.GetLeads(collection.Keys);
        foreach (var cachedLead in cachedLeads)
        {
            collection[cachedLead.Key].Weight = cachedLead.Value.Score;
        }
    }

    private static DateTimeOffset? GetLastCall(ICollection<CallDetailRecord> callRecords)
    {
        var lastUserAnswerAt = callRecords.MaxBy(y => y.UserAnswerAt)?.UserAnswerAt;
        var lastLeadAnswerAt = callRecords.MaxBy(y => y.LeadAnswerAt)?.LeadAnswerAt;

        var lastAnswerAt = lastLeadAnswerAt > lastUserAnswerAt ? lastLeadAnswerAt : lastUserAnswerAt;

        return lastAnswerAt;
    }
}
