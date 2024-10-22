using KL.Manager.API.Persistent.Entities.Cache;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Redis.OM;
using Redis.OM.Searching;

namespace KL.Manager.API.Persistent.Repositories;

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

    public async Task<CallInfoCache?> GetAgentByLead(long clientId, long leadId)
    {
        return await _callInfoCache.FirstOrDefaultAsync(x =>
            x.ClientId == clientId && x.LeadId == leadId);
    }

    public async Task<IDictionary<string, CallInfoCache>> GetCalls(long clientId, IEnumerable<long> agentIds)
    {
        return (await _callInfoCache.Where(x => x.ClientId == clientId).ToListAsync())
            .Where(x => agentIds.Contains(x.AgentId))
            .ToDictionary(x => x.SessionId, x => x);
    }
}
