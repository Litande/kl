using KL.Manager.API.Persistent.Entities.Cache;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Redis.OM;
using Redis.OM.Searching;

namespace KL.Manager.API.Persistent.Repositories;

public class BlockedUserCacheRepository : IBlockedUserCacheRepository
{
    private readonly RedisCollection<BlockedUserCache> _blockedUser;

    public BlockedUserCacheRepository(RedisConnectionProvider redisProvider)
    {
        _blockedUser = (RedisCollection<BlockedUserCache>)redisProvider
            .RedisCollection<BlockedUserCache>(saveState: false);
    }

    public async Task<BlockedUserCache> Add(long userId)
    {
        var cache = new BlockedUserCache
        {
            UserId = userId,
        };

        await _blockedUser.InsertAsync(cache);
        return cache;
    }
}