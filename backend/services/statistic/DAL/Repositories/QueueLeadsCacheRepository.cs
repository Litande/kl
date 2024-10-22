using KL.Statistics.Application.Models;
using Microsoft.EntityFrameworkCore;
using Redis.OM;
using Redis.OM.Searching;

namespace KL.Statistics.DAL.Repositories;

public class QueueLeadsCacheRepository : IQueueLeadsCacheRepository
{
    private readonly RedisCollection<QueueLeadCache> _queueCache;

    public QueueLeadsCacheRepository(RedisConnectionProvider redisProvider)
    {
        _queueCache = (RedisCollection<QueueLeadCache>)redisProvider.RedisCollection<QueueLeadCache>(saveState: false);
    }

    public async Task<IReadOnlyCollection<QueueLeadCache>> GetAll(
        long clientId,
        CancellationToken ct = default)
    {
        var items = await _queueCache
            .Where(r => r.ClientId == clientId)
            .ToArrayAsync(ct);

        return items;
    }
}