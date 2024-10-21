using Plat4Me.DialAgentApi.Application.Enums;
using Plat4Me.DialAgentApi.Application.Models;
using Plat4Me.DialAgentApi.Application.Models.Requests;
using Plat4Me.DialAgentApi.Application.Models.Responses;
using Plat4Me.DialAgentApi.Persistent.Entities;
using Plat4Me.DialAgentApi.Persistent.Entities.Projections;

namespace Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

public interface ILeadRepository
{
    Task<Lead?> GetById(long clientId, long leadId, CancellationToken ct = default);
    Task<Lead?> GetWithDataSource(long clientId, long leadId, CancellationToken ct = default);
    Task<Lead?> SaveFeedbackAndGet(long clientId, long agentId, bool isGenerated, long leadId, LeadSystemStatusTypes? systemStatus, LeadStatusTypes status, DateTimeOffset? remindOn = null, CancellationToken ct = default);
    Task<PaginatedResponse<FutureCallBackProjection>> GetAllFeatureCallBacks(long clientId, long agentId, PaginationRequest pagination, CancellationToken ct = default);
    Task AddLeadHistories(long leadId, LeadHistoryActionType actionType, long? createdBy = null, CancellationToken ct = default, params ValueChanges<object?>[] changes);
}
