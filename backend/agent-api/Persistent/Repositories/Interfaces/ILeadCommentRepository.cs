using Plat4Me.DialAgentApi.Application.Models.Requests;
using Plat4Me.DialAgentApi.Application.Models.Responses;
using Plat4Me.DialAgentApi.Persistent.Entities;
using Plat4Me.DialAgentApi.Persistent.Entities.Projections;

namespace Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

public interface ILeadCommentRepository
{
    Task<PaginatedResponse<LeadCommentProjection>> GetLeadComments(
        long leadId,
        PaginationRequest pagination,
        CancellationToken ct = default);

    Task<LeadComment> AddComment(
        long userId,
        long leadId,
        string comment,
        CancellationToken ct = default);
}