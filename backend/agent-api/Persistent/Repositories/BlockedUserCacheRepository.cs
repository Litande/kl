using KL.Agent.API.Persistent.Entities.Cache;
using KL.Agent.API.Persistent.Repositories.Interfaces;
using Redis.OM;
using Redis.OM.Searching;

namespace KL.Agent.API.Persistent.Repositories;

public class BlockedUserCacheRepository : IBlockedUserCacheRepository
{
    private readonly RedisCollection<BlockedUserCache> _agentCache;

    public BlockedUserCacheRepository(RedisConnectionProvider redisProvider)
    {
        _agentCache =
            (RedisCollection<BlockedUserCache>)redisProvider.RedisCollection<BlockedUserCache>(saveState: false);
    }

    public async Task<bool> IsUserBlocked(string userId)
    {
        return await _agentCache.FindByIdAsync(userId) != null;
    }
}