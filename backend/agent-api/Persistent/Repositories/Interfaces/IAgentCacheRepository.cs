using KL.Agent.API.Persistent.Entities.Cache;

namespace KL.Agent.API.Persistent.Repositories.Interfaces;

public interface IAgentCacheRepository
{
    string LockPrefix { get; }
    Task<AgentStateCache?> GetAgent(long agentId);
    Task<IDictionary<long, AgentStateCache>> GetAllAgents();
    Task UpdateAgent(AgentStateCache agent);
}
