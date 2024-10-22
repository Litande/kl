using KL.Agent.API.Application.Models.Requests;
using KL.Agent.API.Application.Models.Responses;
using KL.Agent.API.Persistent.Entities;
using KL.Agent.API.Persistent.Entities.Projections;

namespace KL.Agent.API.Persistent.Repositories.Interfaces;

public interface ICDRRepository
{
    Task<CallDetailRecord?> GetById(long currentClientId, long cdrId, CancellationToken ct = default);
    Task<CallDetailRecord?> GetBySessionId(string sessionId, CancellationToken ct = default);
    Task<PaginatedResponse<CDRHistoryProjection>> GetAllByUserId(long currentClientId, long currentAgentId, PaginationRequest pagination, CancellationToken ct = default);
    Task<PaginatedResponse<CDRAgentHistoryProjection>> GetAllByLeadPhone(long currentClientId, string phoneNumber, PaginationRequest pagination, CancellationToken ct = default);
    Task<IReadOnlyCollection<CDRFeedbackTimeoutProjection>> GetRecordsForFeedBack(CancellationToken ct = default);
}
