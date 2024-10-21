using Plat4Me.Dial.Statistic.Api.Application.Common.Enums;
using Plat4Me.Dial.Statistic.Api.Application.Common.Extensions;
using Plat4Me.Dial.Statistic.Api.Application.Handlers.Dashboard;
using Plat4Me.Dial.Statistic.Api.Application.Models.Messages;
using Plat4Me.Dial.Statistic.Api.Application.SignalR;
using Plat4Me.Dial.Statistic.Api.DAL.Repositories;

namespace Plat4Me.Dial.Statistic.Api.Application.Handlers.Agent;

public class AgentStatisticsChangeHandler : IAgentStatisticsChangeHandler
{
    private readonly IAgentCacheRepository _agentCacheRepository;
    private readonly ILeadQueueRepository _leadQueueRepository;
    private readonly IQueueLeadsCacheRepository _queueLeadsCacheRepository;
    private readonly IQueueDropRateCacheRepository _queueCacheRepository;
    private readonly IGetAgentsWorkModeHandler _getAgentsWorkModeHandler;
    private readonly ILogger<AgentStatisticsChangeHandler> _logger;
    private readonly IHubSender _hubSender;
    public AgentStatisticsChangeHandler(
        IHubSender hubSender,
        ILogger<AgentStatisticsChangeHandler> logger,
        ILeadQueueRepository leadQueueRepository,
        IQueueLeadsCacheRepository queueLeadsCacheRepository,
        IAgentCacheRepository agentCacheRepository,
        IQueueDropRateCacheRepository queueCacheRepository,
        IGetAgentsWorkModeHandler getAgentsWorkModeHandler)
    {
        _hubSender = hubSender;
        _logger = logger;
        _leadQueueRepository = leadQueueRepository;
        _queueLeadsCacheRepository = queueLeadsCacheRepository;
        _agentCacheRepository = agentCacheRepository;
        _queueCacheRepository = queueCacheRepository;
        _getAgentsWorkModeHandler = getAgentsWorkModeHandler;
    }
    public async Task Process(
        AgentsChangedStatusMessage message,
        CancellationToken ct = default)
    {
        await SendNewGroupStatistic(message, ct);
        await SendNewAgentsWorkMode(message, ct);
    }

    private async Task SendNewGroupStatistic(AgentsChangedStatusMessage message, CancellationToken ct = default)
    {
        var agentIds = message.Commands
            .Where(x => x.AgentStatus
                is AgentStatusTypes.WaitingForTheCall
                or AgentStatusTypes.Offline
                or AgentStatusTypes.InBreak)
            .Select(x => x.AgentId)
            .ToArray();

        if (!agentIds.Any()) return;

        var queueEntities = await _leadQueueRepository.GetEnabledQueuesByAgents(
            message.ClientId,
            agentIds,
            ct);

        if (!queueEntities.Any()) return;

        var queueLeadsCaches = (await _queueLeadsCacheRepository.GetAll(message.ClientId, ct))
            .Where(r => queueEntities.Select(p => p.Id)
                .Contains(r.QueueId))
            .ToArray();

        var onlineAgentIds = (await _agentCacheRepository.GetOnlineAgents()).Select(r => r.AgentId);
        var queueDropRateCaches = await _queueCacheRepository.GetQueueByClient(message.ClientId, ct);
        var leadGroups = queueEntities.ToLeadGroupsResponse(queueLeadsCaches, onlineAgentIds, queueDropRateCaches);

        await _hubSender.SendLeadGroups(message.ClientId, leadGroups, ct);

        _logger.LogInformation("The client Id {ClientId} sent group statistics: {LeadGroups}",
            message.ClientId, string.Join(", ", leadGroups.Select(x => x.Name)));
    }

    private async Task SendNewAgentsWorkMode(AgentsChangedStatusMessage message, CancellationToken ct = default)
    {
        var agentsWorkMode = await _getAgentsWorkModeHandler.Handle(message.ClientId, ct);

        await _hubSender.SendAgentWorkMode(message.ClientId, agentsWorkMode, ct);

        _logger.LogInformation("The client Id {ClientId} sent agents work mode: {AgentsWorkMode}",
            message.ClientId, agentsWorkMode.ToString());
    }
}