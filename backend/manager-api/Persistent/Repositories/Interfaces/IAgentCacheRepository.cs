using Plat4Me.DialClientApi.Persistent.Entities.Cache;

namespace Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

public interface IAgentCacheRepository
{
    Task<AgentStateCache?> GetAgent(long agentId);
    // Task<AgentTrackingCache?> GetAgentByLead(long clientId, long leadId);
    Task<IReadOnlyCollection<AgentStateCache>> GetOnlineAgents();
    Task<IDictionary<long, AgentStateCache>> GetAgents(IEnumerable<long> agentIds);
    Task<IDictionary<long, AgentStateCache>> GetAgents(long clientId);
}
