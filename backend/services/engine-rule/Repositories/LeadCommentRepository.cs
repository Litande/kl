using KL.Engine.Rule.Models.Entities;

namespace KL.Engine.Rule.Repositories;

public class LeadCommentRepository(KlDbContext context) : ILeadCommentRepository
{
    public async Task AddComment(LeadComment comment, CancellationToken ct = default)
    {
        context.LeadComments.Add(comment);
        await context.SaveChangesAsync(ct);
    }
}