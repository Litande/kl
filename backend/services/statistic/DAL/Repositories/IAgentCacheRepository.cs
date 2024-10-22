using KL.Statistics.Application.Models;

namespace KL.Statistics.DAL.Repositories;

public interface IAgentCacheRepository
{
    Task<AgentStateCache?> GetAgent(long agentId);
    Task<IReadOnlyCollection<AgentStateCache>> GetOnlineAgents();
    Task<IDictionary<long, AgentStateCache>> GetAgents(IEnumerable<long> agentIds);
    Task<IDictionary<long, AgentStateCache>> GetAgents(long clientId);
}