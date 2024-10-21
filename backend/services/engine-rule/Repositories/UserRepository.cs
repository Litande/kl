using Microsoft.EntityFrameworkCore;
using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Models.Entities;
using Plat4Me.DialRuleEngine.Application.Repositories;

namespace Plat4Me.DialRuleEngine.Infrastructure.Repositories;

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
