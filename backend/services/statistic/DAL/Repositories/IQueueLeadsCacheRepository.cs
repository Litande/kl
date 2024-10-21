using Plat4Me.Dial.Statistic.Api.Application.Models;

namespace Plat4Me.Dial.Statistic.Api.DAL.Repositories;

public interface IQueueLeadsCacheRepository
{
    Task<IReadOnlyCollection<QueueLeadCache>> GetAll(long clientId, CancellationToken ct = default);
}
