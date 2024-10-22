using KL.Manager.API.Persistent.Entities.Cache;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Redis.OM;
using Redis.OM.Searching;

namespace KL.Manager.API.Persistent.Repositories;

public class QueueDropRateCacheRepository : IQueueDropRateCacheRepository
{
    private readonly RedisCollection<QueueDropRateCache> _queueCaches;

    public QueueDropRateCacheRepository(RedisConnectionProvider redisProvider)
    {
        _queueCaches = (RedisCollection<QueueDropRateCache>)redisProvider.RedisCollection<QueueDropRateCache>(saveState: false);
    }

    public async Task<IDictionary<long, QueueDropRateCache>> GetQueueByClient(long clientId, CancellationToken ct = default)
    {
        var items = await _queueCaches
            .Where(r => r.ClientId == clientId)
            .ToDictionaryAsync(r => r.QueueId, ct);

        return items;
    }
}
