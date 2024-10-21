using Microsoft.EntityFrameworkCore;
using Plat4Me.DialClientApi.Application.Extensions;
using Plat4Me.DialClientApi.Application.Models.Requests;
using Plat4Me.DialClientApi.Application.Models.Responses;
using Plat4Me.DialClientApi.Application.Models.Responses.Leads;
using Plat4Me.DialClientApi.Persistent.Entities.Cache;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;
using Redis.OM;
using Redis.OM.Searching;

namespace Plat4Me.DialClientApi.Persistent.Repositories;

public class QueueLeadsCacheRepository : RepositoryBase, IQueueLeadsCacheRepository
{
    private readonly RedisCollection<QueueLeadCache> _queueCache;

    public QueueLeadsCacheRepository(RedisConnectionProvider redisProvider)
    {
        _queueCache = (RedisCollection<QueueLeadCache>)redisProvider.RedisCollection<QueueLeadCache>(saveState: false);
    }

    public async Task<IReadOnlyCollection<QueueLeadCache>> GetQueue(
        long clientId,
        long queueId,
        CancellationToken ct = default)
    {
        var item = await _queueCache
            .Where(r => r.ClientId == clientId
                        && r.QueueId == queueId)
            .ToArrayAsync(ct);

        return item;
    }

    public async Task<PaginatedResponse<LeadItem>> GetLeads(
        long clientId,
        long queueId,
        PaginationRequest pagination,
        CancellationToken ct = default)
    {
        var leads = await _queueCache
            .Where(x => x.ClientId == clientId
                        && x.QueueId == queueId
            )
            .ToArrayAsync(ct);

        return leads.ToLeadsPaginateResponse(pagination);
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

    public async Task<QueueLeadCache?> Get(long leadId)
    {
        return await _queueCache.FindByIdAsync(leadId.ToString());
    }
}