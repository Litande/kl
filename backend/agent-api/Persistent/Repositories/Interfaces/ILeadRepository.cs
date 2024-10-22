using KL.Agent.API.Application.Enums;
using KL.Agent.API.Application.Models;
using KL.Agent.API.Application.Models.Requests;
using KL.Agent.API.Application.Models.Responses;
using KL.Agent.API.Persistent.Entities;
using KL.Agent.API.Persistent.Entities.Projections;

namespace KL.Agent.API.Persistent.Repositories.Interfaces;

public interface ILeadRepository
{
    Task<Lead?> GetById(long clientId, long leadId, CancellationToken ct = default);
    Task<Lead?> GetWithDataSource(long clientId, long leadId, CancellationToken ct = default);
    Task<Lead?> SaveFeedbackAndGet(long clientId, long agentId, bool isGenerated, long leadId, LeadSystemStatusTypes? systemStatus, LeadStatusTypes status, DateTimeOffset? remindOn = null, CancellationToken ct = default);
    Task<PaginatedResponse<FutureCallBackProjection>> GetAllFeatureCallBacks(long clientId, long agentId, PaginationRequest pagination, CancellationToken ct = default);
    Task AddLeadHistories(long leadId, LeadHistoryActionType actionType, long? createdBy = null, CancellationToken ct = default, params ValueChanges<object?>[] changes);
}
