using KL.Engine.Rule.Models;
using Redis.OM;
using Redis.OM.Searching;

namespace KL.Engine.Rule.Repositories;

public class LeadLastCacheRepository : ILeadLastCacheRepository
{
    private readonly RedisCollection<LeadTrackingCache> _leadLastCache;

    public LeadLastCacheRepository(RedisConnectionProvider redisProvider)
    {
        _leadLastCache = (RedisCollection<LeadTrackingCache>)redisProvider.RedisCollection<LeadTrackingCache>(saveState: false);
    }

    public async Task<LeadTrackingCache?> GetLead(long leadId)
    {
        return await _leadLastCache.FindByIdAsync(leadId.ToString());
    }

    public async Task<ICollection<LeadTrackingCache>> GetLeads(IEnumerable<long> leads)
    {
        return (await _leadLastCache.FindByIdsAsync(
                        leads.Select(x => x.ToString())
                    ))
                    .Where(x => x.Value is not null)
                    .Select(x => x.Value!)
                    .ToArray();
    }

    public async Task UpdateScore(long leadId, long? score)
    {
        var leadCache = await GetLead(leadId);

        if (leadCache is null)
            leadCache = new LeadTrackingCache(leadId, score);
        else
            leadCache.Score = score;

        await _leadLastCache.UpdateAsync(leadCache);
    }

    public async Task UpdateLeads(IDictionary<long, long> leadsScores)
    {
        var cachedLeads = (await GetLeads(leadsScores.Select(x => x.Key)))
            .ToDictionary(x => x.LeadId, x => x);

        foreach (var lead in leadsScores)
        {
            var isLeadCached = cachedLeads.TryGetValue(lead.Key, out var cachedLead);

            if (isLeadCached)
            {
                cachedLead!.Score = lead.Value;
            }
            else
                cachedLead = new LeadTrackingCache(lead.Key, lead.Value);

            await _leadLastCache.UpdateAsync(cachedLead);
        }
    }
}
