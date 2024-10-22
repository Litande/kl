using KL.Agent.API.Persistent.Entities;
using KL.Agent.API.Persistent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KL.Agent.API.Persistent.Repositories;

public class UserRepository : RepositoryBase, IUserRepository
{
    private readonly DialDbContext _context;

    public UserRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetById(long clientId, long userId, CancellationToken ct = default)
    {
        var entity = await _context.Users
            .Where(r => r.ClientId == clientId
                        && r.UserId == userId)
            .FirstOrDefaultAsync(ct);

        return entity;
    }

    public async Task SetOfflineSince(
        long clientId,
        long userId,
        DateTimeOffset? offlineSince,
        CancellationToken ct = default)
    {
        var user = await GetById(clientId, userId, ct);
        if (user is not null)
        {
            user.OfflineSince = offlineSince;
            await _context.SaveChangesAsync(ct);
        }
    }
}
