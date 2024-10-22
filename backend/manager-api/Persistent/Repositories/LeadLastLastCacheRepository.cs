using KL.Manager.API.Persistent.Entities.Cache;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Redis.OM;
using Redis.OM.Searching;

namespace KL.Manager.API.Persistent.Repositories;

public class LeadLastLastCacheRepository : ILeadLastCacheRepository
{
    private readonly RedisCollection<LeadTrackingCache> _leadLastCache;

    public LeadLastLastCacheRepository(RedisConnectionProvider redisProvider)
    {
        _leadLastCache = (RedisCollection<LeadTrackingCache>)redisProvider.RedisCollection<LeadTrackingCache>(saveState: false);
    }

    public async Task<LeadTrackingCache?> GetLead(long leadId)
    {
        return await _leadLastCache.FindByIdAsync(leadId.ToString());
    }

    public async Task<Dictionary<long, LeadTrackingCache>> GetLeads(IEnumerable<long> leadIds)
    {
        return (await _leadLastCache.FindByIdsAsync(
                leadIds.Select(x => x.ToString())
            ))
            .Where(x => x.Value is not null)
            .Select(x => x.Value!)
            .ToDictionary(x => x.LeadId, x => x);
    }
}
