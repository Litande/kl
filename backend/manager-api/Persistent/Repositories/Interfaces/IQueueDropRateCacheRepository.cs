using KL.Manager.API.Persistent.Entities.Cache;

namespace KL.Manager.API.Persistent.Repositories.Interfaces;

public interface IQueueDropRateCacheRepository
{
    Task<IDictionary<long, QueueDropRateCache>> GetQueueByClient(long clientId, CancellationToken ct = default);
}
