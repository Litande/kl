using KL.Caller.Leads.Enums;
using KL.Caller.Leads.Models;
using KL.Caller.Leads.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KL.Caller.Leads.Repositories;

public class UserRepository(KlDbContext context) : IUserRepository
{
    public async Task<IDictionary<long, AgentScore>> GetAgentsWithScore(
        long clientId,
        CancellationToken ct = default)
    {
        var q = context.Users.ActiveAgents(clientId);

        return await q
            .Select(x => new
            {
                AgentId = x.Id,
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
        var user = await context.Users
            .Where(x => x.ClientId == clientId
                        && x.Id == userId)
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
