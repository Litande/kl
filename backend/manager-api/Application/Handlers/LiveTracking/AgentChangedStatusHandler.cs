using KL.Manager.API.Application.Extensions;
using KL.Manager.API.Application.Models.Messages.Agents;
using KL.Manager.API.Application.Services.Interfaces;
using KL.Manager.API.Persistent.Repositories.Interfaces;

namespace KL.Manager.API.Application.Handlers.LiveTracking;

public class AgentChangedStatusHandler : IAgentChangedStatusHandler
{
    private readonly ILeadRepository _leadRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHubSender _hubSender;
    private readonly IAgentCacheRepository _agentCacheRepository;
    private readonly ICallInfoCacheRepository _callInfoCacheRepository;
    private readonly ILeadQueueRepository _leadQueueRepository;
    private readonly IQueueLeadsCacheRepository _queueLeadsCacheRepository;

    public AgentChangedStatusHandler(
        ILeadRepository leadRepository,
        IUserRepository userRepository,
        IAgentCacheRepository agentCacheRepository,
        ICallInfoCacheRepository callInfoCacheRepository,
        ILeadQueueRepository leadQueueRepository,
        IQueueLeadsCacheRepository queueLeadsCacheRepository,
        IHubSender hubSender)
    {
        _leadRepository = leadRepository;
        _userRepository = userRepository;
        _agentCacheRepository = agentCacheRepository;
        _callInfoCacheRepository = callInfoCacheRepository;
        _leadQueueRepository = leadQueueRepository;
        _queueLeadsCacheRepository = queueLeadsCacheRepository;
        _hubSender = hubSender;
    }

    public async Task Process(
        AgentChangedStatusMessage message,
        CancellationToken ct = default)
    {
        var agent = await _userRepository.GetAgentInfoById(message.ClientId, message.AgentId, ct);
        if (agent is null)
            return;

        var leadQueueId = (await _queueLeadsCacheRepository.GetAll(message.ClientId, ct))
            .Where(r => r.LeadId == message.CallInfo?.LeadId)
            .Select(r => r.QueueId)
            .FirstOrDefault();

        var leadQueue = leadQueueId != 0
            ? (await _leadQueueRepository
                .GetEnabledQueue(message.ClientId, leadQueueId, ct))
            : null;

        var lead = message.CallInfo?.LeadId is not null
            ? await _leadRepository.GetLeadInfoById(message.ClientId, message.CallInfo.LeadId.Value, ct)
            : null;

        var agentCache = await _agentCacheRepository.GetAgent(message.AgentId);
        var callInfo =  !string.IsNullOrEmpty(message.CallInfo?.SessionId)
            ? await _callInfoCacheRepository.GetCallInfo(message.CallInfo.SessionId)
            : null;

        var agentsTracking = AgentTrackingExtensions.Map(agent, agentCache, callInfo, lead, leadQueue);

        await _hubSender.SendAgentsList(message.ClientId, new[] { agentsTracking }, ct);
    }
}
