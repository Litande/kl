using KL.Statistics.Application.Models;

namespace KL.Statistics.DAL.Repositories;

public interface IQueueLeadsCacheRepository
{
    Task<IReadOnlyCollection<QueueLeadCache>> GetAll(long clientId, CancellationToken ct = default);
}
