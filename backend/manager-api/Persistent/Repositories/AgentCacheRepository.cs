using Microsoft.EntityFrameworkCore;
using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Persistent.Entities.Cache;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;
using Redis.OM;
using Redis.OM.Searching;

namespace Plat4Me.DialClientApi.Persistent.Repositories;

public class AgentCacheRepository : IAgentCacheRepository
{
    private readonly RedisCollection<AgentStateCache> _agentCache;

    public AgentCacheRepository(RedisConnectionProvider redisProvider)
    {
        _agentCache = (RedisCollection<AgentStateCache>)redisProvider.RedisCollection<AgentStateCache>(saveState: false);
    }

    public async Task<AgentStateCache?> GetAgent(long agentId)
    {
        return await _agentCache.FindByIdAsync(agentId.ToString());
    }

    public async Task<IReadOnlyCollection<AgentStateCache>> GetOnlineAgents()
    {
        return (await _agentCache.ToListAsync())
            .Where(x => x.AgentStatus != AgentInternalStatusTypes.Offline
                        && x.AgentDisplayStatus != AgentStatusTypes.Offline)
            .ToArray();
    }

    public async Task<IDictionary<long, AgentStateCache>> GetAgents(IEnumerable<long> agentIds)
    {
        return (await _agentCache.FindByIdsAsync(agentIds.Select(x => x.ToString())))
            .Where(x => x.Value is not null)
            .ToDictionary(x => x.Value!.AgentId, x => x.Value!);
    }

    public async Task<IDictionary<long, AgentStateCache>> GetAgents(long clientId)
    {
        return await _agentCache
            .Where(x => x.ClientId == clientId)
            .ToDictionaryAsync(x => x.AgentId, x => x);
    }
}
