using KL.Engine.Rule.App;
using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Models;
using KL.Engine.Rule.Models.Entities;
using KL.Engine.Rule.Repositories;
using KL.Engine.Rule.Services.Contracts;
using Microsoft.Extensions.Options;

namespace KL.Engine.Rule.Services;

public class LeadsQueueStore : ILeadsQueueStore
{
    private readonly ILeadQueueRepository _leadQueueRepository;

    private IDictionary<long, Dictionary<long, LeadQueue>> _clientQueues =
        new Dictionary<long, Dictionary<long, LeadQueue>>();

    private IDictionary<long, List<long>> _queueLeadIds = new Dictionary<long, List<long>>();

    private IDictionary<long, Dictionary<long, TrackedLead>> _clientLeads =
        new Dictionary<long, Dictionary<long, TrackedLead>>();

    private IDictionary<long, long> _leadManualScores = new Dictionary<long, long>();
    private readonly FutureQueueOptions _futureQueueOptions;

    public LeadsQueueStore(
        ILeadQueueRepository leadQueueRepository,
        IOptions<FutureQueueOptions> options)
    {
        _leadQueueRepository = leadQueueRepository;
        _futureQueueOptions = options.Value;

        UpdateQueues();
        InitQueueLeads();
    }

    private void UpdateQueues()
    {
        _clientQueues = _leadQueueRepository.GetAll()
            .GroupBy(r => r.ClientId)
            .ToDictionary(
                r => r.Key,
                r => r.ToDictionary(p => p.Id));
    }

    private void InitQueueLeads()
    {
        var queueIds = _clientQueues
            .SelectMany(r => r.Value.Select(p => p.Value.Id))
            .Distinct();

        _queueLeadIds = queueIds
            .ToDictionary(
                r => r,
                _ => new List<long>());
    }

    public void UpdateAll(long clientId, IReadOnlyCollection<TrackedLead> leads)
    {
        if (!leads.Any()) return;

        _clientLeads[clientId] = leads
            .Where(r => r.LeadQueueId.HasValue)
            .ToDictionary(r => r.LeadId);

        ApplyLeadsManualScores(clientId);
        UpdateLeadsQueues(clientId);
    }

    private void UpdateLeadsQueues(long clientId)
    {
        if (!_clientQueues.TryGetValue(clientId, out var queues))
        {
            UpdateQueues();
            queues = _clientQueues[clientId]; // will throw exception
        }

        if (!_clientLeads.TryGetValue(clientId, out var clientLeads))
        {
            clientLeads = new Dictionary<long, TrackedLead>();
            _clientLeads[clientId] = clientLeads;
        }

        var queueLeads = clientLeads
            .GroupBy(r => r.Value.LeadQueueId!.Value)
            .ToDictionary(r => r.Key, r => r.Select(x => x.Value));

        foreach (var (queueId, queue) in queues)
        {
            if (queueLeads.TryGetValue(queueId, out var leads))
            {
                var isFutureQueue = queue.Type is LeadQueueTypes.Future;
                leads = isFutureQueue
                    ? leads.OrderBy(r => r.RemindOn)
                        .ThenByDescending(r => r.Score)
                    : leads.OrderByDescending(r => r.Score);
            }

            _queueLeadIds[queueId] = leads?.Select(r => r.LeadId).ToList() ?? new List<long>();
        }
    }

    public void SetLeadManualScore(long clientId, long leadId, long score)
    {
        _leadManualScores.Add(leadId, score);
        ApplyLeadsManualScores(clientId);
        UpdateLeadsQueues(clientId);
    }

    private void ApplyLeadsManualScores(long clientId)
    {
        var clientLeads = _clientLeads[clientId];
        foreach (var (leadId, score) in _leadManualScores)
        {
            if (!clientLeads.ContainsKey(leadId))
                continue;

            clientLeads[leadId].Score = score;
        }
    }

    public TrackedLead? PopNextLead(
        long clientId,
        long queueId,
        IReadOnlyCollection<long>? agentIds)
    {
        var isQueueExists = _queueLeadIds.TryGetValue(queueId, out var leadIds);
        if (!isQueueExists || leadIds is null) return null;

        var leadId = TryGetLeadId(clientId, leadIds, agentIds);
        if (leadId is null) return null;

        leadIds.Remove(leadId.Value);

        if (_leadManualScores.ContainsKey(leadId.Value))
            _leadManualScores.Remove(leadId.Value);

        return _clientLeads[clientId][leadId.Value];
    }

    public bool TryRemoveLead(long clientId, long leadId)
    {
        var isSuccess = _clientLeads[clientId].Remove(leadId);

        if (isSuccess)
        {
            ApplyLeadsManualScores(clientId);
            UpdateLeadsQueues(clientId);
        }

        return isSuccess;
    }

    private long? TryGetLeadId(
        long clientId,
        IReadOnlyCollection<long> leadIds,
        IReadOnlyCollection<long>? agentIds)
    {
        if (!leadIds.Any()) return null;

        var clientLeads = _clientLeads[clientId];
        var dateRange = DateTimeOffset.UtcNow.AddMinutes(_futureQueueOptions.AmountMinutesToStartCall);

        if (agentIds?.Any() == true)
        {
            foreach (var agentId in agentIds)
            {
                var assignedLead = leadIds
                    .FirstOrDefault(leadId => clientLeads[leadId].AssignedAgentId == agentId
                                              && clientLeads[leadId].RemindOn <= dateRange);
                if (assignedLead != 0)
                    return assignedLead;
            }
        }

        var unAssignedLead = leadIds
            .FirstOrDefault(leadId => !clientLeads[leadId].AssignedAgentId.HasValue);

        if (unAssignedLead == 0)
            return null;

        return unAssignedLead;
    }
}