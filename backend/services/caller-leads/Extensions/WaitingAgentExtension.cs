using KL.Caller.Leads.Models;

namespace KL.Caller.Leads.Extensions;

public static class WaitingAgentExtension
{
    public static IEnumerable<WaitingAgent> SortAgentsByScore(
        this IEnumerable<WaitingAgent> waitingAgents,
        IDictionary<long, AgentScore> agentScores)
    {
        return waitingAgents
            .OrderByDescending(x => agentScores[x.AgentId].Score);
    }
}
