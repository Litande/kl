using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Models;

namespace KL.Engine.Rule.Repositories;

public class QueueLeadsCacheRepository : IQueueLeadsCacheRepository
{
    private readonly RedisCollection<QueueLeadCache> _queueLeadCache;
    private readonly ILeadRepository _leadRepository;

    public QueueLeadsCacheRepository(RedisConnectionProvider redisProvider, ILeadRepository leadRepository)
    {
        _queueLeadCache =
            (RedisCollection<QueueLeadCache>)redisProvider.RedisCollection<QueueLeadCache>(saveState: false);
        _leadRepository = leadRepository;
    }

    public async Task<IReadOnlyCollection<QueueLeadCache>> GetAll(
        long clientId,
        CancellationToken ct = default)
    {
        var items = await _queueLeadCache
            .Where(r => r.ClientId == clientId)
            .ToArrayAsync(ct);

        return items;
    }

    public async Task<QueueLeadCache?> GetById(
        long clientId,
        long queueId,
        long leadId)
    {
        var item = await _queueLeadCache
            .FirstOrDefaultAsync(r => r.ClientId == clientId
                                      && r.QueueId == queueId
                                      && r.LeadId == leadId);

        return item;
    }

    public async Task<QueueLeadCache?> GetById(
        long clientId,
        long leadId)
    {
        var item = await _queueLeadCache
            .FirstOrDefaultAsync(r => r.ClientId == clientId
                                      && r.LeadId == leadId);

        return item;
    }

    public async Task<long?> GetQueueId(long clientId, long leadId)
    {
        var item = await _queueLeadCache
            .FirstOrDefaultAsync(r => r.ClientId == clientId
                                      && r.LeadId == leadId);

        return item?.QueueId;
    }

    public async Task Remove(
        long clientId,
        long? queueId,
        long leadId,
        CancellationToken ct = default)
    {
        QueueLeadCache? leadCache;
        if (queueId.HasValue)
            leadCache = await GetById(clientId, queueId.Value, leadId);
        else
            leadCache = await GetById(clientId, leadId);

        if (leadCache is null) return;

        await _queueLeadCache.DeleteAsync(leadCache);
    }

    public async Task UpdateAll(
        long clientId,
        IEnumerable<TrackedLead> leadsUpdate,
        CancellationToken ct = default)
    {
        await RemoveUnused(clientId, ct);

        foreach (var item in leadsUpdate
                     .Where(r => r.LeadQueueId.HasValue))
        {
            await _queueLeadCache.InsertAsync(new QueueLeadCache
            {
                ClientId = clientId,
                QueueId = item.LeadQueueId!.Value,
                LeadId = item.LeadId,
                Score = item.Score,
                Status = item.Status,
                SystemStatus = item.SystemStatus,
                RemindOn = item.RemindOn
            });
        }
    }

    public async Task UpdateSystemStatus(
        long clientId,
        long queueId,
        long leadId,
        LeadSystemStatusTypes? systemStatus)
    {
        var item = await GetById(clientId, queueId, leadId);
        if (item is null) return;

        item.SystemStatus = systemStatus;
        await _queueLeadCache.UpdateAsync(item);
    }

    public async Task UpdateScore(
        long clientId,
        long queueId,
        long leadId,
        long score)
    {
        var item = await GetById(clientId, queueId, leadId);
        if (item is null) return;

        item.Score = score;
        await _queueLeadCache.UpdateAsync(item);
    }

    private async Task RemoveUnused(long clientId, CancellationToken ct = default)
    {
        var items = await GetAll(clientId, ct);

        var itemsToRemove = items.Where(r => !r.SystemStatus.HasValue);
        await _queueLeadCache.DeleteAsync(itemsToRemove);
    }

    public async Task ValidateLeadCache(IEnumerable<LeadStatusDto> leads, CancellationToken ct = default)
    {
        var cachedLeads = await _queueLeadCache
            .ToDictionaryAsync(x => x.LeadId, ct);

        
        //overwrite cache from db
        foreach (var lead in leads)
        {
            if (cachedLeads.TryGetValue(lead.LeadId, out var item))
            {
                cachedLeads.Remove(item.LeadId);
                item.Status = lead.Status;
                item.SystemStatus = lead.SystemStatus;
                item.RemindOn = lead.RemindOn;
                await _queueLeadCache.UpdateAsync(item);
            }
            // else
            // {
            //     item = new QueueLeadCache()
            //     {
            //         ClientId = lead.ClientId,
            //         LeadId = lead.LeadId,
            //         QueueId = 0,
            //         Score = 0,
            //         Status = lead.Status,
            //         SystemStatus = lead.SystemStatus,
            //         RemindOn = lead.RemindOn
            //     };
            //     await _queueLeadCache.UpdateAsync(item);
            // }

        }

        //remove cache for missing leads and leads with empty systemstatus
        //queued leads should be restored by leadprocessing pipelines
        await _queueLeadCache.DeleteAsync(cachedLeads.Values);
    }
}
