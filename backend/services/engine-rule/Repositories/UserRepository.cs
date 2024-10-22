using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KL.Engine.Rule.Repositories;

public class UserRepository(KlDbContext context) : IUserRepository
{
    public async Task<IReadOnlyCollection<User>> GetOfflineAgentsSince(
        DateTimeOffset fromDate,
        CancellationToken ct = default)
    {
        return await context.Users
            .Where(x => x.RoleType == RoleTypes.Agent
                        && x.OfflineSince <= fromDate)
            .ToArrayAsync(ct);
    }
}
