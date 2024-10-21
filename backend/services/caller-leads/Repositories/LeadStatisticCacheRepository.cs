using Plat4Me.DialLeadCaller.Application.Models.LeadStatisticCache;
using Plat4Me.DialLeadCaller.Application.Repositories;
using Redis.OM;
using Redis.OM.Searching;

namespace Plat4Me.DialLeadCaller.Infrastructure.Repositories;

public class LeadStatisticCacheRepository : ILeadStatisticCacheRepository
{
    private readonly RedisCollection<LeadStatisticCache> _leadStatisticCaches;

    public LeadStatisticCacheRepository(RedisConnectionProvider redisProvider)
    {
        _leadStatisticCaches =
            (RedisCollection<LeadStatisticCache>)redisProvider.RedisCollection<LeadStatisticCache>(saveState: false);
    }

    public async Task UpdateStatistics(long clientId, List<StatisticItemCache> leadsStatisticCaches)
    {
        var statistic = new LeadStatisticCache
        {
            ClientId = clientId,
            Statistics = leadsStatisticCaches
        };
        await _leadStatisticCaches.UpdateAsync(statistic);
    }

    public async Task<LeadStatisticCache?> GetLeadStatisticByClient(long clientId)
    {
        return await _leadStatisticCaches.FindByIdAsync(clientId.ToString());
    }
}