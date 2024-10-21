using Plat4Me.Dial.Statistic.Api.Application.Models;

namespace Plat4Me.Dial.Statistic.Api.DAL.Repositories;

public interface IAgentCacheRepository
{
    Task<AgentStateCache?> GetAgent(long agentId);
    Task<IReadOnlyCollection<AgentStateCache>> GetOnlineAgents();
    Task<IDictionary<long, AgentStateCache>> GetAgents(IEnumerable<long> agentIds);
    Task<IDictionary<long, AgentStateCache>> GetAgents(long clientId);
}