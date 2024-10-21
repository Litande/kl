using Plat4Me.DialRuleEngine.Application.Models.Entities;
using Plat4Me.DialRuleEngine.Application.Repositories;

namespace Plat4Me.DialRuleEngine.Infrastructure.Repositories;

public class LeadCommentRepository : ILeadCommentRepository
{
    private readonly DialDbContext _context;

    public LeadCommentRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task AddComment(LeadComment comment, CancellationToken ct = default)
    {
        _context.LeadComments.Add(comment);
        await _context.SaveChangesAsync(ct);
    }
}