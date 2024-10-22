using KL.Manager.API.Application.Models.Requests;
using KL.Manager.API.Application.Models.Responses;
using KL.Manager.API.Persistent.Entities.Projections;

namespace KL.Manager.API.Persistent.Repositories.Interfaces;

public interface ILeadBlacklistRepository
{
    Task<PaginatedResponse<LeadBlacklistProjection>> GetAll(long clientId, PaginationRequest pagination, CancellationToken ct = default);
    Task Create(long clientId, long userId, long leadId, CancellationToken ct = default);
    Task<bool> Delete(long clientId, IEnumerable<long> leadIds, CancellationToken ct = default);
}
