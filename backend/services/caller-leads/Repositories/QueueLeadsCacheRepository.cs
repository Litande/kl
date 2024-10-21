using Plat4Me.DialLeadCaller.Application.Enums;
using Plat4Me.DialLeadCaller.Application.Models;
using Plat4Me.DialLeadCaller.Application.Repositories;
using Redis.OM;
using Redis.OM.Searching;

namespace Plat4Me.DialLeadCaller.Infrastructure.Repositories;

public class QueueLeadsCacheRepository : IQueueLeadsCacheRepository
{
    private readonly RedisCollection<QueueLeadCache> _queueCache;

    public QueueLeadsCacheRepository(RedisConnectionProvider redisProvider)
    {
        _queueCache = (RedisCollection<QueueLeadCache>)redisProvider.RedisCollection<QueueLeadCache>(saveState: false);
    }

    public async Task<QueueLeadCache?> GetById(
        long clientId,
        long queueId,
        long leadId)
    {
        var item = await _queueCache
            .FirstOrDefaultAsync(r => r.ClientId == clientId
                                      && r.QueueId == queueId
                                      && r.LeadId == leadId);

        return item;
    }

    public async Task UpdateStatus(
        long clientId,
        long queueId,
        long leadId,
        LeadSystemStatusTypes? systemStatus,
        LeadStatusTypes? status = null,
        CancellationToken ct = default)
    {
        var item = await GetById(clientId, queueId, leadId);
        if (item is null) return;

        item.SystemStatus = systemStatus;
        if (status is not null)
            item.Status = status.Value;

        await _queueCache.UpdateAsync(item);
    }
}
