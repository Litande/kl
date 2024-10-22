using KL.Statistics.Application.Models.Entities;

namespace KL.Statistics.DAL.Repositories;

public interface ICdrRepository
{
    Task<CallDetailRecord?> GetById(long currentClientId, long cdrId, CancellationToken ct = default);
    Task<IReadOnlyCollection<CallDetailRecord>> GetCallsByPeriod(long clientId, DateTimeOffset fromDateTime, DateTimeOffset toDateTime, CancellationToken ct = default);
}
