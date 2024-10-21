using Plat4Me.DialClientApi.Persistent.Entities.Cache;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;
using Redis.OM;
using Redis.OM.Searching;

namespace Plat4Me.DialClientApi.Persistent.Repositories;

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