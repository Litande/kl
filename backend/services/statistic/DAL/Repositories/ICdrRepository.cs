using Plat4Me.Dial.Statistic.Api.Application.Models.Entities;

namespace Plat4Me.Dial.Statistic.Api.DAL.Repositories;

public interface ICdrRepository
{
    Task<CallDetailRecord?> GetById(long currentClientId, long cdrId, CancellationToken ct = default);
    Task<IReadOnlyCollection<CallDetailRecord>> GetCallsByPeriod(long clientId, DateTimeOffset fromDateTime, DateTimeOffset toDateTime, CancellationToken ct = default);
}
