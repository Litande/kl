using KL.Agent.API.Application.Extensions;
using KL.Agent.API.Application.Models.Requests;
using KL.Agent.API.Application.Models.Responses;
using KL.Agent.API.Persistent.Entities;
using KL.Agent.API.Persistent.Entities.Projections;
using KL.Agent.API.Persistent.Repositories.Interfaces;

namespace KL.Agent.API.Persistent.Repositories;

public class LeadCommentRepository : RepositoryBase, ILeadCommentRepository
{
    private readonly DialDbContext _context;

    public LeadCommentRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<LeadCommentProjection>> GetLeadComments(
        long leadId,
        PaginationRequest pagination,
        CancellationToken ct = default)
    {
        var q = _context.LeadComments
            .Where(x => x.LeadId == leadId)
            .Select(x => new LeadCommentProjection
            {
                Id = x.Id,
                AgentId = x.CreatedById,
                AgentFullName = (x.CreatedBy.FirstName.Trim() + " " + x.CreatedBy.LastName.Trim()).Trim(),
                Comment = x.Comment,
                CreatedAt = x.CreatedAt,
                LeadStatus = x.Lead.Status.ToDescription(),
            });

        return await CreatePaginatedResponse(q, pagination, ct);
    }

    public async Task<LeadComment> AddComment(
        long userId,
        long leadId,
        string comment,
        CancellationToken ct = default)
    {
        var leadComment = new LeadComment
        {
            CreatedById = userId,
            LeadId = leadId,
            Comment = comment,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        await _context.LeadComments.AddAsync(leadComment, ct);
        await _context.SaveChangesAsync(ct);

        return leadComment;
    }
}