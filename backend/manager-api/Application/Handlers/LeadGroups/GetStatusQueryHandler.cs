using Plat4Me.DialClientApi.Application.Extensions;
using Plat4Me.DialClientApi.Application.Models.Responses.LeadGroups;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialClientApi.Application.Handlers.LeadGroups;

public class GetStatusQueryHandler : IStatusQueryHandler
{
    private readonly ILeadQueueRepository _leadQueueRepository;
    private readonly IAgentCacheRepository _agentCacheRepository;
    private readonly IQueueLeadsCacheRepository _queueLeadsCacheRepository;
    private readonly IQueueDropRateCacheRepository _queueCacheRepository;

    public GetStatusQueryHandler(
        ILeadQueueRepository leadQueueRepository,
        IAgentCacheRepository agentCacheRepository,
        IQueueLeadsCacheRepository queueLeadsCacheRepository,
        IQueueDropRateCacheRepository queueCacheRepository)
    {
        _leadQueueRepository = leadQueueRepository;
        _agentCacheRepository = agentCacheRepository;
        _queueLeadsCacheRepository = queueLeadsCacheRepository;
        _queueCacheRepository = queueCacheRepository;
    }

    public async Task<IReadOnlyCollection<LeadGroup>> Handle(
        long clientId,
        CancellationToken ct = default)
    {
        var queueLeadsCache = await _queueLeadsCacheRepository.GetAll(clientId, ct);

        var queueEntities = await _leadQueueRepository.GetEnabledQueuesWithAgents(clientId, ct);
        var onlineAgentIds = (await _agentCacheRepository.GetOnlineAgents()).Select(r => r.AgentId);
        var queueDropRateCaches = await _queueCacheRepository.GetQueueByClient(clientId, ct);

        return queueEntities.ToLeadGroupsResponse(queueLeadsCache, onlineAgentIds, queueDropRateCaches);
    }
}
