using Plat4Me.Dial.Statistic.Api.Application.Common.Enums;
using Plat4Me.Dial.Statistic.Api.Application.Models.Responses;
using Plat4Me.Dial.Statistic.Api.DAL.Repositories;

namespace Plat4Me.Dial.Statistic.Api.Application.Handlers.Dashboard;

public class GetAgentsWorkModeHandler : IGetAgentsWorkModeHandler
{
    private readonly IAgentCacheRepository _agentCacheRepository;

    public GetAgentsWorkModeHandler(IAgentCacheRepository agentCacheRepository)
    {
        _agentCacheRepository = agentCacheRepository;
    }

    public async Task<AgentsWorkMode> Handle(long clientId, CancellationToken ct = default)
    {
        var statuses = new HashSet<AgentStatusTypes>
        {
            AgentStatusTypes.WaitingForTheCall,
            AgentStatusTypes.InTheCall,
            AgentStatusTypes.FillingFeedback,
            AgentStatusTypes.InBreak,
        };

        var agents = await _agentCacheRepository.GetAgents(clientId);
        var agentStatusAndCount = agents
            .Select(x => x.Value)
            .Where(x => statuses.Contains(x.AgentDisplayStatus))
            .GroupBy(x => x.AgentDisplayStatus)
            .ToDictionary(x => x.Key, x => x.Count());

        foreach (var status in statuses)
        {
            agentStatusAndCount.TryAdd(status, 0);
        }

        return new AgentsWorkMode(
            agentStatusAndCount.Sum(x => x.Value),
            agentStatusAndCount[AgentStatusTypes.InTheCall],
            agentStatusAndCount[AgentStatusTypes.WaitingForTheCall],
            agentStatusAndCount[AgentStatusTypes.FillingFeedback],
            agentStatusAndCount[AgentStatusTypes.InBreak]
        );
    }
}