using Plat4Me.DialAgentApi.Persistent.Entities.Cache;
using Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;
using Redis.OM;
using Redis.OM.Searching;

namespace Plat4Me.DialAgentApi.Persistent.Repositories;

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
