using KL.Agent.API.Persistent.Entities.Cache;
using KL.Agent.API.Persistent.Repositories.Interfaces;
using Redis.OM;
using Redis.OM.Searching;

namespace KL.Agent.API.Persistent.Repositories;

public class LeadCacheRepository : ILeadCacheRepository
{
    private readonly RedisCollection<LeadTrackingCache> _leadCache;

    public LeadCacheRepository(RedisConnectionProvider redisProvider)
    {
        _leadCache = (RedisCollection<LeadTrackingCache>)redisProvider.RedisCollection<LeadTrackingCache>(saveState: false);
    }

    public async Task<LeadTrackingCache?> GetLead(long leadId)
    {
        return await _leadCache.FindByIdAsync(leadId.ToString());
    }

    public async Task<Dictionary<long, LeadTrackingCache>> GetLeads(IEnumerable<long> leadIds)
    {
        return (await _leadCache.FindByIdsAsync(
                        leadIds.Select(x => x.ToString())
                    ))
                    .Where(x => x.Value is not null)
                    .Select(x => x.Value!)
                    .ToDictionary(x => x.LeadId, x => x);
    }
}
