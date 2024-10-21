using Plat4Me.DialClientApi.Application.Models.Requests;
using Plat4Me.DialClientApi.Application.Models.Requests.CallRecords;
using Plat4Me.DialClientApi.Application.Models.Responses;
using Plat4Me.DialClientApi.Persistent.Entities;
using Plat4Me.DialClientApi.Persistent.Entities.Projections;

namespace Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

public interface ICDRRepository
{
    Task<CallDetailRecord?> GetById(long currentClientId, long cdrId, CancellationToken ct = default);
    Task<PaginatedResponse<CDRProjection>> ListCalls(long currentClientId, PaginationRequest pagination, CDRFilterRequest? filter = null, CancellationToken ct = default);
    Task<IReadOnlyCollection<CallDetailRecord>> GetCallsByPeriod(long clientId, DateTimeOffset fromDateTime, DateTimeOffset toDateTime, CancellationToken ct = default);
}
