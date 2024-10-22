using KL.Caller.Leads.Models;
using Microsoft.EntityFrameworkCore;
using Redis.OM;
using Redis.OM.Searching;

namespace KL.Caller.Leads.Repositories;

public class QueueDropRateCacheRepository : IQueueDropRateCacheRepository
{
    private readonly RedisCollection<QueueDropRateCache> _queueCaches;

    public QueueDropRateCacheRepository(RedisConnectionProvider redisProvider)
    {
        _queueCaches = (RedisCollection<QueueDropRateCache>)redisProvider.RedisCollection<QueueDropRateCache>(saveState: false);
    }

    public async Task<IDictionary<long, QueueDropRateCache>> GetQueueByClient(
        long clientId,
        CancellationToken ct = default)
    {
        var items = await Queryable.Where(_queueCaches, r => r.ClientId == clientId)
            .ToDictionaryAsync(r => r.QueueId, ct);

        return items;
    }

    public async Task Update(long clientId, long queueId, double dropRate)
    {
        var item = await _queueCaches.FindByIdAsync(queueId.ToString())
                   ?? new QueueDropRateCache { ClientId = clientId, QueueId = queueId };

        item.DropRate = dropRate;
        await _queueCaches.UpdateAsync(item);
    }
}
