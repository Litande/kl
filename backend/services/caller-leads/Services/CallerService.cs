using KL.Caller.Leads.Clients;
using KL.Caller.Leads.Enums;
using KL.Caller.Leads.Extensions;
using KL.Caller.Leads.Handlers.Contracts;
using KL.Caller.Leads.Models;
using KL.Caller.Leads.Models.Messages;
using KL.Caller.Leads.Models.Requests;
using KL.Caller.Leads.Models.Responses;
using KL.Caller.Leads.Repositories;
using KL.Caller.Leads.Services.Contracts;

namespace KL.Caller.Leads.Services;

public class CallerService : ICallerService
{
    private readonly ILogger<CallerService> _logger;
    private readonly ILeadRuleEngineClient _leadStoreClient;
    private readonly IPublishForCallHandler _publishForCallHandler;
    private readonly ILeadQueueRepository _leadQueueRepository;
    private readonly IAgentStateRepository _agentStateStore;
    private readonly IUserRepository _userRepository;
    private readonly ILeadRepository _leadRepository;
    private readonly IBridgeService _bridgeService;
    private readonly ISettingsService _settingsService;
    private readonly ISipProviderService _sipProviderService;

    public CallerService(
        ILogger<CallerService> logger,
        ILeadRuleEngineClient leadStoreClient,
        IPublishForCallHandler publishForCallHandler,
        ILeadQueueRepository leadQueueRepository,
        IAgentStateRepository agentStateStore,
        IUserRepository userRepository,
        ILeadRepository leadRepository,
        IBridgeService bridgeService,
        ISipProviderService sipProviderService,
        ISettingsService settingsService)
    {
        _logger = logger;
        _leadStoreClient = leadStoreClient;
        _publishForCallHandler = publishForCallHandler;
        _leadQueueRepository = leadQueueRepository;
        _agentStateStore = agentStateStore;
        _userRepository = userRepository;
        _leadRepository = leadRepository;
        _bridgeService = bridgeService;
        _settingsService = settingsService;
        _sipProviderService = sipProviderService;
    }

    public async Task TryToCallPredictive(CancellationToken ct = default)
    {
        if (!_bridgeService.AnyBridgeRegistered())
        {
            _logger.LogError("No available bridges");
            return;
        }

        var clientsQueues = await GetClientsLeadQueues(ct);
        var clientIds = clientsQueues.Select(x => x.Key).ToArray();
        var clientsDialerSettings = await _settingsService.GetProductiveDialerSettingsOrDefault(clientIds, ct);
        var clientRtcSettings = await _settingsService.GetRtcSettings(clientIds, ct);

        foreach (var clientQueue in clientsQueues)
        {
            var clientId = clientQueue.Key;
            var clientAgents = await GetClientWaitingAgentsOrdered(clientId, ct);

            foreach (var leadQueue in clientQueue)
            {
                var queueAgents = clientAgents.Where(r => leadQueue.AssignedAgentIds.Contains(r.AgentId))
                    .ToArray();
                _logger.LogInformation("The client id {clientId} / queue id {queueId} has agent ids {agentIds} waiting for a call",
                    clientId, leadQueue.Id, string.Join(", ", queueAgents.Select(r => r.AgentId)));

                if (!queueAgents.Any()) continue;

                var queueCallsLimit = (int)Math.Ceiling(leadQueue.Ratio * queueAgents.Count());
                var maxLeadsForAgent = (int)Math.Floor(leadQueue.Ratio);

                _logger.LogInformation("queueAgents AgentId / LeadsToCallCount - {queueAgents}",
                    string.Join(", ", queueAgents.Select(r => $"{r.AgentId} / {r.CallsCount}")));
                _logger.LogInformation("queueCallsLimit {queueCallsLimit}", queueCallsLimit);
                _logger.LogInformation("maxLeadsForAgent {maxLeadsForAgent}", maxLeadsForAgent);

                var callToRequests = new List<CallToRequest>();
                while (queueCallsLimit > callToRequests.Count)
                {
                    var bridgeId = _bridgeService.GetBridge();
                    if (bridgeId is null)
                    {
                        _logger.LogWarning("No available bridges while TryToCallPredictive");
                        break;
                    }

                    var queueLimitAgentIds = queueAgents
                        .Where(r => r.CallsCount < maxLeadsForAgent)
                        .Select(r => r.AgentId)
                        .ToArray();

                    if (!queueLimitAgentIds.Any()) break;
                    var lead = await GetNextLead(clientId, leadQueue.Id, queueLimitAgentIds, ct);
                    if (lead is null) break;

                    var isFixedAssigned = lead.AssignedAgentId is not null;
                    if (isFixedAssigned)
                        _logger.LogInformation("The agent Id {agentId} assigned to lead Id {leadId}",
                            lead.AssignedAgentId, lead.LeadId);

                    var agent = isFixedAssigned
                        ? queueAgents.First(r => r.AgentId == lead.AssignedAgentId)
                        : queueAgents.First(r => r.AgentId == queueLimitAgentIds.First());

                    agent.CallsCount += 1;

                    var sipProvider = await _sipProviderService.GetProviderForPredictiveCall(ct);
                    if (sipProvider is null)
                    {
                        _logger.LogWarning("SipProvider not found");
                        continue;
                    }

                    callToRequests.Add(
                        new CallToRequest(clientId,
                            bridgeId,
                            CallType.Predictive,
                            leadQueue.Id,
                            lead.LeadId,
                            isFixedAssigned,
                            lead.LeadPhone,
                            agent.AgentId,
                            clientsDialerSettings[clientId].RingingTimeout,
                            clientsDialerSettings[clientId].MaxCallDuration,
                            clientRtcSettings[clientId]?.IceServers,
                            lead.IsTest,
                            sipProvider.MapToInfo()));

                    if (ct.IsCancellationRequested) break;
                }

                if (!callToRequests.Any()) continue;

                _logger.LogInformation("There are agent Id / lead Id {items} items for a call a prepared",
                    string.Join(", ", callToRequests.Select(r => $"{r.AgentId} / {r.LeadId}")));

                var agentIdsToCall = callToRequests
                    .Select(r => r.AgentId)
                    .Distinct();

                await _agentStateStore.RemoveAgents(
                    await _agentStateStore.GetWaitingAgentsForClient(clientId, ct));

                await PublishCallAndUpdateSystemStatus(clientId, callToRequests, ct);
            }
        }
    }

    private async Task<IReadOnlyCollection<WaitingAgentWrapper>> GetClientWaitingAgentsOrdered(
        long clientId,
        CancellationToken ct = default)
    {
        var agentsWithScore = await _userRepository.GetAgentsWithScore(clientId, ct);
        var queueAgents = (await _agentStateStore.GetWaitingAgentsForClient(clientId,  ct))
            .SortAgentsByScore(agentsWithScore)
            .Select(r => new WaitingAgentWrapper(r.AgentId, 0))
            .ToArray();

        return queueAgents;
    }

    // private async Task UpdateAgentStatus(
    //     long clientId,
    //     IEnumerable<long> agentIds,
    //     CancellationToken ct = default)
    // {

    //     var commands = agentIds
    //         .Select(r =>
    //             new AgentChangedStatusCommand(
    //                 AgentId: r,
    //                 AgentStatus: AgentStatusTypes.Dialing))
    //         .ToArray();

    //     var changedStatusMessage = new AgentsChangedStatusMessage(clientId, commands);
    //     await _agentsUpdateStatusHandler.Process(changedStatusMessage, ct);
    // }

    private async Task PublishCallAndUpdateSystemStatus(
        long clientId,
        IReadOnlyCollection<CallToRequest> callToRequests,
        CancellationToken ct = default)
    {
        var leadIds = callToRequests.Select(r => r.LeadId!.Value);
        await _leadRepository.UpdateSystemStatuses(clientId, leadIds, LeadSystemStatusTypes.Dialing, ct);

        await _publishForCallHandler.Process(callToRequests, ct);
    }

    public async Task TryToCallManual(ManualCallMessage message, CancellationToken ct = default)
    {
        var bridgeId = _bridgeService.GetBridge();
        if (bridgeId is null)
        {
            _logger.LogError("No available bridges");
            return;
        }

        var dialerSettings = await _settingsService.GetProductiveDialerSettingsOrDefault(message.ClientId, ct);
        var rtcSettings = await _settingsService.GetRtcSettings(message.ClientId, ct);
        var sipProvider = await _sipProviderService.GetProviderForManualCall(ct);
        if (sipProvider is null)
        {
            _logger.LogError("SipProvider not found for clientId {ClientId}", message.ClientId);
            return;
        }

        var request = new CallToRequest(
            message.ClientId,
            bridgeId,
            CallType.Manual,
            LeadQueueId: null,
            LeadId: null,
            IsFixedAssigned: false,
            message.Phone,
            message.AgentId,
            dialerSettings.RingingTimeout,
            dialerSettings.MaxCallDuration,
            rtcSettings?.IceServers,
            IsTest: false,
            sipProvider.MapToInfo());

        await _publishForCallHandler.Process(request, ct);
    }

    public async Task TryRecall(
        long clientId,
        long agentId,
        long leadQueueId,
        long leadId,
        long providerId,
        string leadPhone,
        bool isFixedAssigned,
        bool isTest,
        CancellationToken ct = default)
    {
        var bridgeId = _bridgeService.GetBridge();
        if (bridgeId is null)
        {
            _logger.LogError("No available bridges");
            return;
        }

        var dialerSettings = await _settingsService.GetProductiveDialerSettingsOrDefault(clientId, ct);
        var rtcSettings = await _settingsService.GetRtcSettings(clientId, ct);

        var sipProvider = await _sipProviderService.GetProviderForRecall(providerId, ct);
        if (sipProvider is null)
        {
            _logger.LogWarning("SipProvider not found");
            return;
        }

        var request = new CallToRequest(
            clientId,
            bridgeId,
            CallType.Predictive,
            leadQueueId,
            leadId,
            isFixedAssigned,
            leadPhone,
            agentId,
            dialerSettings.RingingTimeout,
            dialerSettings.MaxCallDuration,
            rtcSettings?.IceServers,
            isTest,
            sipProvider.MapToInfo());

        await _publishForCallHandler.Process(request, ct);
    }

    public async Task<long?> GetFreeAgentId(
        long clientId,
        long queueId,
        long agentId,
        long? leadId,
        CancellationToken ct = default)
    {
        var queue = await _leadQueueRepository.GetWithAgents(queueId, ct);
        if (queue is null)
            return null;

        var assignedAgentIds = queue.AssignedAgentIds.Where(r => r != agentId);
        var clientAgents = await GetClientWaitingAgentsOrdered(clientId, ct);
        var waitingAgent = clientAgents.FirstOrDefault(r => assignedAgentIds.Contains(r.AgentId));

        if (waitingAgent is null)
            return null;

        await _agentStateStore.RemoveAgent(waitingAgent.AgentId);        

        return waitingAgent.AgentId;
    }

    private async Task<GetNextLeadResponse?> GetNextLead(
        long clientId,
        long queueId,
        IEnumerable<long> waitingAgentIds,
        CancellationToken ct = default)
    {
        var request = new GetNextLeadRequest(queueId, waitingAgentIds);
        var response = await _leadStoreClient.GetNextLead(clientId, request, ct);

        if (response is not null) return response;

        _logger.LogInformation("There are currently no leads in the queue Id: {queueId}", queueId);
        return null;
    }

    private async Task<IReadOnlyCollection<IGrouping<long, LeadQueueAgents>>> GetClientsLeadQueues(
        CancellationToken ct = default)
    {
        var items = await _leadQueueRepository.GetAllWithAgentsOrdered(ct);

        return items
            .GroupBy(r => r.ClientId)
            .ToArray();
    }
}
