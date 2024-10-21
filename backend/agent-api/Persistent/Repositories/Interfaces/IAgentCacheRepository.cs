using Plat4Me.DialAgentApi.Persistent.Entities.Cache;

namespace Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

public interface IAgentCacheRepository
{
    string LockPrefix { get; }
    Task<AgentStateCache?> GetAgent(long agentId);
    Task<IDictionary<long, AgentStateCache>> GetAllAgents();
    Task UpdateAgent(AgentStateCache agent);
}
