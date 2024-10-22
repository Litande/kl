using KL.Manager.API.Application.Models.Requests;
using KL.Manager.API.Application.Models.Requests.CallRecords;
using KL.Manager.API.Application.Models.Responses;
using KL.Manager.API.Persistent.Entities;
using KL.Manager.API.Persistent.Entities.Projections;

namespace KL.Manager.API.Persistent.Repositories.Interfaces;

public interface ICDRRepository
{
    Task<CallDetailRecord?> GetById(long currentClientId, long cdrId, CancellationToken ct = default);
    Task<PaginatedResponse<CDRProjection>> ListCalls(long currentClientId, PaginationRequest pagination, CDRFilterRequest? filter = null, CancellationToken ct = default);
    Task<IReadOnlyCollection<CallDetailRecord>> GetCallsByPeriod(long clientId, DateTimeOffset fromDateTime, DateTimeOffset toDateTime, CancellationToken ct = default);
}
