using Plat4Me.DialClientApi.Application.Extensions;
using Plat4Me.DialClientApi.Application.Models.Responses.AgentTrackings;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialClientApi.Application.Handlers.LiveTracking;

public class GetAgentsQueryHandler : IAgentsQueryHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IAgentCacheRepository _agentCacheRepository;
    private readonly ICallInfoCacheRepository _callInfoCacheRepository;
    private readonly ILeadRepository _leadRepository;
    private readonly IQueueLeadsCacheRepository _queueLeadsCacheRepository;
    private readonly ILeadQueueRepository _leadQueueRepository;

    public GetAgentsQueryHandler(
        IUserRepository userRepository,
        IAgentCacheRepository agentCacheRepository,
        ICallInfoCacheRepository callInfoCacheRepository,
        ILeadRepository leadRepository,
        IQueueLeadsCacheRepository queueLeadsCacheRepository,
        ILeadQueueRepository leadQueueRepository)
    {
        _userRepository = userRepository;
        _agentCacheRepository = agentCacheRepository;
        _callInfoCacheRepository = callInfoCacheRepository;
        _leadRepository = leadRepository;
        _queueLeadsCacheRepository = queueLeadsCacheRepository;
        _leadQueueRepository = leadQueueRepository;
    }

    public async Task<IEnumerable<AgentTrackingResponse>> Handle(
        long clientId,
        CancellationToken ct = default)
    {
        var agents = await _userRepository.GetAgentInfoByClientId(clientId, ct);
        var agentIds = agents.Select(r => r.AgentId);
        var agentCaches = await _agentCacheRepository.GetAgents(agentIds);
        var callInfoCaches = await _callInfoCacheRepository.GetCalls(clientId, agentIds);

        var leadIds = callInfoCaches
            .Where(r => r.Value.LeadId.HasValue)
            .Select(x => x.Value.LeadId!.Value);

        var leadAndQueueIds = (await _queueLeadsCacheRepository.GetAll(clientId, ct))
            .Where(r => leadIds.Contains(r.LeadId))
            .ToDictionary(r => r.LeadId, r => r.QueueId);

        var queueIds = leadAndQueueIds.Values.Distinct().ToArray();

        var leadQueues = (await _leadQueueRepository
                .GetEnabledQueues(clientId, queueIds, ct))
            .ToDictionary(x => x.Id, x => x);

        var leadAndQueue = leadAndQueueIds.ToDictionary(x => x.Key, x => leadQueues[x.Value]);

        var leads = await _leadRepository.GetLeadInfoByIds(clientId, leadIds, ct);

        var agentsTracking = agents.Select(r =>
            AgentTrackingExtensions.Map(r, agentCaches, callInfoCaches, leads, leadAndQueue));

        return agentsTracking;
    }
}
