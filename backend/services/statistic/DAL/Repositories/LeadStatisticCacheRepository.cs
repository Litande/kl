﻿using KL.Statistics.Application.Models.StatisticCache;
using Redis.OM;
using Redis.OM.Searching;

namespace KL.Statistics.DAL.Repositories;

public class LeadStatisticCacheRepository : ILeadStatisticCacheRepository
{
    private readonly RedisCollection<StatisticCache> _leadStatisticCaches;

    public LeadStatisticCacheRepository(RedisConnectionProvider redisProvider)
    {
        _leadStatisticCaches = (RedisCollection<StatisticCache>)redisProvider.RedisCollection<StatisticCache>(saveState: false);
    }

    public async Task<StatisticCache?> GetLeadStatisticByClient(long clientId)
    {
        return await _leadStatisticCaches.FindByIdAsync(clientId.ToString());
    }
}
