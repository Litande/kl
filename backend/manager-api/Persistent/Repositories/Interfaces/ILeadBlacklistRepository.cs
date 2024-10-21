using Plat4Me.DialClientApi.Application.Models.Requests;
using Plat4Me.DialClientApi.Application.Models.Responses;
using Plat4Me.DialClientApi.Persistent.Entities.Projections;

namespace Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

public interface ILeadBlacklistRepository
{
    Task<PaginatedResponse<LeadBlacklistProjection>> GetAll(long clientId, PaginationRequest pagination, CancellationToken ct = default);
    Task Create(long clientId, long userId, long leadId, CancellationToken ct = default);
    Task<bool> Delete(long clientId, IEnumerable<long> leadIds, CancellationToken ct = default);
}
