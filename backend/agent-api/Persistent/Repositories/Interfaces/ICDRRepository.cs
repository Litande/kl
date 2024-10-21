using Plat4Me.DialAgentApi.Application.Models.Requests;
using Plat4Me.DialAgentApi.Application.Models.Responses;
using Plat4Me.DialAgentApi.Persistent.Entities;
using Plat4Me.DialAgentApi.Persistent.Entities.Projections;

namespace Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

public interface ICDRRepository
{
    Task<CallDetailRecord?> GetById(long currentClientId, long cdrId, CancellationToken ct = default);
    Task<CallDetailRecord?> GetBySessionId(string sessionId, CancellationToken ct = default);
    Task<PaginatedResponse<CDRHistoryProjection>> GetAllByUserId(long currentClientId, long currentAgentId, PaginationRequest pagination, CancellationToken ct = default);
    Task<PaginatedResponse<CDRAgentHistoryProjection>> GetAllByLeadPhone(long currentClientId, string phoneNumber, PaginationRequest pagination, CancellationToken ct = default);
    Task<IReadOnlyCollection<CDRFeedbackTimeoutProjection>> GetRecordsForFeedBack(CancellationToken ct = default);
}
