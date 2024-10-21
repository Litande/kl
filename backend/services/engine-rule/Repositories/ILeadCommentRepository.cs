using Plat4Me.DialRuleEngine.Application.Models.Entities;

namespace Plat4Me.DialRuleEngine.Application.Repositories;

public interface ILeadCommentRepository
{
    Task AddComment(LeadComment comment, CancellationToken ct = default);
}