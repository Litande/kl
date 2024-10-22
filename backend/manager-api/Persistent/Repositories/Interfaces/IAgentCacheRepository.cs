using KL.Manager.API.Persistent.Entities.Cache;

namespace KL.Manager.API.Persistent.Repositories.Interfaces;

public interface IAgentCacheRepository
{
    Task<AgentStateCache?> GetAgent(long agentId);
    // Task<AgentTrackingCache?> GetAgentByLead(long clientId, long leadId);
    Task<IReadOnlyCollection<AgentStateCache>> GetOnlineAgents();
    Task<IDictionary<long, AgentStateCache>> GetAgents(IEnumerable<long> agentIds);
    Task<IDictionary<long, AgentStateCache>> GetAgents(long clientId);
}
