using KL.Caller.Leads.Models;

namespace KL.Caller.Leads.Repositories;

public interface IAgentStateRepository
{
    Task<IReadOnlyCollection<WaitingAgent>> GetWaitingAgentsForClient(long clientId, CancellationToken ct = default);
    Task<WaitingAgent?> GetWaitingAgentById(long clientId, long agentId, CancellationToken ct = default);
    Task UpdateAgents(IEnumerable<WaitingAgent> agents);
    Task UpdateAgent(WaitingAgent agent);
    Task RemoveAgents(IEnumerable<WaitingAgent> agents, CancellationToken ct = default);
    Task RemoveAgent(WaitingAgent agent, CancellationToken ct = default);
    Task RemoveAgent(long id, CancellationToken ct = default);
}
