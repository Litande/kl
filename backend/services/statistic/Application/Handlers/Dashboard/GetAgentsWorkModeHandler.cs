using KL.Statistics.Application.Common.Enums;
using KL.Statistics.Application.Models.Responses;
using KL.Statistics.DAL.Repositories;

namespace KL.Statistics.Application.Handlers.Dashboard;

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