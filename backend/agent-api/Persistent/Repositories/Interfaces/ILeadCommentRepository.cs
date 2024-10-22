using KL.Agent.API.Application.Models.Requests;
using KL.Agent.API.Application.Models.Responses;
using KL.Agent.API.Persistent.Entities;
using KL.Agent.API.Persistent.Entities.Projections;

namespace KL.Agent.API.Persistent.Repositories.Interfaces;

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