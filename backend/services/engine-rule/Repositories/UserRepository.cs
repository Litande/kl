using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Models.Entities;

namespace KL.Engine.Rule.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DialDbContext _context;

    public UserRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<User>> GetOfflineAgentsSince(
        DateTimeOffset fromDate,
        CancellationToken ct = default)
    {
        return await _context.Users
            .Where(x => x.RoleType == RoleTypes.Agent
                        && x.OfflineSince <= fromDate)
            .ToArrayAsync(ct);
    }
}
