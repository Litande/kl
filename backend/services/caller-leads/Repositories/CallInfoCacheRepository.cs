using KL.Caller.Leads.Models;

namespace KL.Caller.Leads.Repositories;

public class CallInfoCacheRepository : ICallInfoCacheRepository
{
    private readonly RedisCollection<CallInfoCache> _callInfoCache;
    const string CALLINFO_LOCK_PREFIX = "callinfo_lock_";
    public string LockPrefix => CALLINFO_LOCK_PREFIX;

    public CallInfoCacheRepository(RedisConnectionProvider redisProvider)
    {
        _callInfoCache = (RedisCollection<CallInfoCache>)redisProvider.RedisCollection<CallInfoCache>(saveState: false);
    }

    public async Task<CallInfoCache?> GetCallInfo(string sessionId)
    {
        return await _callInfoCache.FindByIdAsync(sessionId);
    }


    public async Task UpdateCallInfo(CallInfoCache cache)
    {
        await _callInfoCache.UpdateAsync(cache);
    }

    public async Task IncreaseCallAgain(string sessionId)
    {
        var callCache = await _callInfoCache.FindByIdAsync(sessionId);

        if (callCache is not null)
        {
            callCache.CallAgainCount = callCache.CallAgainCount.HasValue
                ? callCache.CallAgainCount + 1
                : 1;

            await _callInfoCache.UpdateAsync(callCache);
        }
    }
    
    public async Task RemoveCallInfo(string sessionId)
    {
        var callCache = await _callInfoCache.FindByIdAsync(sessionId);
        if (callCache is not null)
            await _callInfoCache.DeleteAsync(callCache);
    }
}
