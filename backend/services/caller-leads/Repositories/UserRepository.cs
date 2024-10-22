using KL.Caller.Leads.Enums;
using KL.Caller.Leads.Models;
using KL.Caller.Leads.Models.Entities;

namespace KL.Caller.Leads.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DialDbContext _context;

    public UserRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task<IDictionary<long, AgentScore>> GetAgentsWithScore(
        long clientId,
        CancellationToken ct = default)
    {
        var q = _context.Users.ActiveAgents(clientId);

        return await q
            .Select(x => new
            {
                AgentId = x.UserId,
                Score = x.UserTags
                    .Where(p => p.Tag.Status == TagStatusTypes.Enable
                                && (!p.ExpiredOn.HasValue
                                    || p.ExpiredOn >= DateTimeOffset.UtcNow))
                    .Sum(p => p.Tag.Value)
            })
            .ToDictionaryAsync(
                x => x.AgentId,
                x => new AgentScore(x.AgentId, x.Score),
                ct);
    }

    public async Task<User?> Get(long clientId, long userId, CancellationToken ct = default)
    {
        var user = await _context.Users
            .Where(x => x.ClientId == clientId
                        && x.UserId == userId)
            .FirstOrDefaultAsync(ct);

        return user;
    }
}

internal static class UserRepositoryExtensions
{
    public static IQueryable<User> ActiveAgents(
        this IQueryable<User> context,
        long clientId)
    {
        var q = context
            .Where(r => r.ClientId == clientId
                        && !r.DeletedAt.HasValue
                        && r.RoleType == RoleTypes.Agent
                        && r.Status == UserStatusTypes.Active);
        return q;
    }
}
