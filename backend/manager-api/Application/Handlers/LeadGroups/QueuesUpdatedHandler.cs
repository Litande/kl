using KL.Manager.API.Application.Extensions;
using KL.Manager.API.Application.Models.Messages.LeadGroups;
using KL.Manager.API.Application.Services.Interfaces;
using KL.Manager.API.Persistent.Entities;
using KL.Manager.API.Persistent.Entities.Cache;
using KL.Manager.API.Persistent.Repositories.Interfaces;

namespace KL.Manager.API.Application.Handlers.LeadGroups;

public class QueuesUpdatedHandler : IQueuesUpdatedHandler
{
    private readonly ILeadQueueRepository _leadQueueRepository;
    private readonly ILogger<QueuesUpdatedHandler> _logger;
    private readonly IAgentCacheRepository _agentCacheRepository;
    private readonly IQueueLeadsCacheRepository _queueLeadsCacheRepository;
    private readonly IQueueDropRateCacheRepository _queueCacheRepository;
    private readonly IHubSender _hubSender;

    public QueuesUpdatedHandler(
        ILogger<QueuesUpdatedHandler> logger,
        ILeadQueueRepository leadQueueRepository,
        IAgentCacheRepository agentCacheRepository,
        IQueueLeadsCacheRepository queueLeadsCacheRepository,
        IQueueDropRateCacheRepository queueCacheRepository,
        IHubSender hubSender)
    {
        _logger = logger;
        _leadQueueRepository = leadQueueRepository;
        _agentCacheRepository = agentCacheRepository;
        _queueLeadsCacheRepository = queueLeadsCacheRepository;
        _queueCacheRepository = queueCacheRepository;
        _hubSender = hubSender;
    }

    public async Task Process(QueuesUpdatedMessage message, CancellationToken ct = default)
    {
        var filterQueueIds = message.QueueIds ?? Array.Empty<long>();
        var queuesEntities = await _leadQueueRepository.GetEnabledQueues(message.ClientId, filterQueueIds, ct);
        var queueLeadsCaches = await _queueLeadsCacheRepository.GetAll(message.ClientId, ct);

        if (filterQueueIds.Any())
            queueLeadsCaches = queueLeadsCaches
                .Where(r => filterQueueIds.Contains(r.QueueId))
                .ToArray();

        await LeadQueuesSend(message.ClientId, queueLeadsCaches, queuesEntities, ct);
        await LeadGroupsSend(message.ClientId, queueLeadsCaches, queuesEntities, ct);
    }

    private async Task LeadQueuesSend(
        long clientId,
        IEnumerable<QueueLeadCache> queueLeadsCaches,
        IEnumerable<LeadQueue> queuesEntities,
        CancellationToken ct = default)
    {
        var leadQueues = queuesEntities.ToLeadQueuesResponse(queueLeadsCaches);

        await _hubSender.SendLeadsQueue(clientId, leadQueues, ct);

        _logger.LogInformation("The client Id {ClientId} sent leads queue statistics: {LeadQueues}",
            clientId, string.Join(", ", leadQueues.Select(x => x.GroupName)));
    }

    private async Task LeadGroupsSend(
        long clientId,
        IEnumerable<QueueLeadCache> queueLeadsCaches,
        IEnumerable<LeadQueue> leadQueuesEntities,
        CancellationToken ct = default)
    {
        var onlineAgentIds = (await _agentCacheRepository.GetOnlineAgents()).Select(r => r.AgentId);
        var queueDropRateCaches = await _queueCacheRepository.GetQueueByClient(clientId, ct);
        var leadGroups = leadQueuesEntities.ToLeadGroupsResponse(queueLeadsCaches, onlineAgentIds, queueDropRateCaches);

        await _hubSender.SendLeadGroups(clientId, leadGroups, ct);

        _logger.LogInformation("The client Id {ClientId} sent group statistics: {LeadGroups}",
            clientId, string.Join(", ", leadGroups.Select(x => x.Name)));
    }
}