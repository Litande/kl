using Plat4Me.DialClientApi.Persistent.Entities.Cache;

namespace Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

public interface IQueueDropRateCacheRepository
{
    Task<IDictionary<long, QueueDropRateCache>> GetQueueByClient(long clientId, CancellationToken ct = default);
}
