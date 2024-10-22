using KL.Statistics.Application.Models;

namespace KL.Statistics.DAL.Repositories;

public interface IQueueDropRateCacheRepository
{
    Task<IDictionary<long, QueueDropRateCache>> GetQueueByClient(long clientId, CancellationToken ct = default);
}
