using KL.Engine.Rule.Models.Entities;

namespace KL.Engine.Rule.Repositories;

public interface ILeadCommentRepository
{
    Task AddComment(LeadComment comment, CancellationToken ct = default);
}