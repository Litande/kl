using Microsoft.EntityFrameworkCore;
using Plat4Me.DialAgentApi.Persistent.Entities.Cache;
using Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;
using Redis.OM;
using Redis.OM.Searching;

namespace Plat4Me.DialAgentApi.Persistent.Repositories;

public class AgentCacheRepository : IAgentCacheRepository
{
    const string AGENTCACHE_LOCK_PREFIX = "agentcache_lock_";
    public string LockPrefix => AGENTCACHE_LOCK_PREFIX;
    private readonly RedisCollection<AgentStateCache> _agentCache;

    public AgentCacheRepository(RedisConnectionProvider redisProvider)
    {
        _agentCache = (RedisCollection<AgentStateCache>)redisProvider.RedisCollection<AgentStateCache>(saveState: false);
    }

    public async Task<AgentStateCache?> GetAgent(long agentId)
    {
        return await _agentCache.FindByIdAsync(agentId.ToString());
    }

    public async Task<IDictionary<long, AgentStateCache>> GetAllAgents()
    {
        return await _agentCache.ToDictionaryAsync(x => x.AgentId, x => x);
    }

    public async Task UpdateAgent(AgentStateCache agent)
    {
        await _agentCache.UpdateAsync(agent);
    }
}
