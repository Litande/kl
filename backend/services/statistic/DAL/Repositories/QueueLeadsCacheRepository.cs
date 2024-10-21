using Microsoft.EntityFrameworkCore;
using Plat4Me.Dial.Statistic.Api.Application.Models;
using Redis.OM;
using Redis.OM.Searching;

namespace Plat4Me.Dial.Statistic.Api.DAL.Repositories;

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