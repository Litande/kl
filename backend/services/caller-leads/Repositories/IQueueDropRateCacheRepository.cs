using Plat4Me.DialLeadCaller.Application.Models;

namespace Plat4Me.DialLeadCaller.Application.Repositories;

public interface IQueueDropRateCacheRepository
{
    Task<IDictionary<long, QueueDropRateCache>> GetQueueByClient(long clientId, CancellationToken ct = default);
    Task Update(long clientId, long queueId, double dropRate);
}
