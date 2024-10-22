using KL.Agent.API.Persistent.Entities.Cache;
using KL.Agent.API.Persistent.Repositories.Interfaces;
using Redis.OM;
using Redis.OM.Searching;

namespace KL.Agent.API.Persistent.Repositories;

public class CallInfoCacheRepository : ICallInfoCacheRepository
{
    private readonly RedisCollection<CallInfoCache> _callInfoCache;

    public CallInfoCacheRepository(RedisConnectionProvider redisProvider)
    {
        _callInfoCache = (RedisCollection<CallInfoCache>)redisProvider.RedisCollection<CallInfoCache>(saveState: false);
    }

    public async Task<CallInfoCache?> GetCallInfo(string sessionId)
    {
        return await _callInfoCache.FindByIdAsync(sessionId);
    }

}
