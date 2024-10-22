using KL.Caller.Leads.Models;

namespace KL.Caller.Leads.Repositories;

public interface IQueueDropRateCacheRepository
{
    Task<IDictionary<long, QueueDropRateCache>> GetQueueByClient(long clientId, CancellationToken ct = default);
    Task Update(long clientId, long queueId, double dropRate);
}
