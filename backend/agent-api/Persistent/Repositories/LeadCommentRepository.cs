using Plat4Me.DialAgentApi.Application.Extensions;
using Plat4Me.DialAgentApi.Application.Models.Requests;
using Plat4Me.DialAgentApi.Application.Models.Responses;
using Plat4Me.DialAgentApi.Persistent.Entities;
using Plat4Me.DialAgentApi.Persistent.Entities.Projections;
using Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialAgentApi.Persistent.Repositories;

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