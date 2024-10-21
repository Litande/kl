using Microsoft.EntityFrameworkCore;
using Plat4Me.DialAgentApi.Persistent.Entities.Cache;
using Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;
using Redis.OM;
using Redis.OM.Searching;

namespace Plat4Me.DialAgentApi.Persistent.Repositories;

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