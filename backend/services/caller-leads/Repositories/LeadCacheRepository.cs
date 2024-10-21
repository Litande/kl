using Plat4Me.DialLeadCaller.Application.Repositories;
using Plat4Me.DialLeadCaller.Application.Models;
using Redis.OM.Searching;
using Redis.OM;

namespace Plat4Me.DialLeadCaller.Infrastructure.Repositories;

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

    public async Task<ICollection<LeadTrackingCache>> GetLeads(IEnumerable<long> leads)
    {
        return (await _leadCache.FindByIdsAsync(leads.Select(x => x.ToString())))
            .Where(x => x.Value is not null)
            .Select(x => x.Value!)
            .ToArray();
    }
}