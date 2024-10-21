using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Application.Models.Responses.Team;
using Plat4Me.DialClientApi.Persistent.Entities;
using Plat4Me.DialClientApi.Persistent.Entities.Cache;

namespace Plat4Me.DialClientApi.Application.Extensions;

public static class TeamExtensions
{
    public static TeamItemResponse ToResponse(this Team team) =>
        new(team.Id,
            team.Name);

    public static TeamExtraInfoResponse ToResponse(this Team team, IDictionary<long, AgentStateCache> agentCaches) =>
        new(team.Id,
            team.Name,
            team.Agents.Count,
            GetCountOnlineAgents(team, agentCaches),
            GetCountOfflineAgents(team, agentCaches));

    private static int GetCountOnlineAgents(Team team, IDictionary<long, AgentStateCache> agentCaches)
    {
        return agentCaches
            .Count(x => team.Agents
                            .Any(y => y.UserId == x.Value.AgentId)
                        && x.Value.AgentStatus != AgentInternalStatusTypes.Offline 
                        && x.Value.AgentDisplayStatus != AgentStatusTypes.Offline
                    );
    }

    private static int GetCountOfflineAgents(Team team, IDictionary<long, AgentStateCache> agentCaches)
    {
        return team.Agents.Count - GetCountOnlineAgents(team, agentCaches);
    }
}
