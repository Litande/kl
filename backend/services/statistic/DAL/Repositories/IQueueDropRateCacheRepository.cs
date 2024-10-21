using Plat4Me.Dial.Statistic.Api.Application.Models;

namespace Plat4Me.Dial.Statistic.Api.DAL.Repositories;

public interface IQueueDropRateCacheRepository
{
    Task<IDictionary<long, QueueDropRateCache>> GetQueueByClient(long clientId, CancellationToken ct = default);
}
