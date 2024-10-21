using Microsoft.EntityFrameworkCore;
using Plat4Me.DialLeadCaller.Application.Enums;
using Plat4Me.DialLeadCaller.Application.Models.Entities;
using Plat4Me.DialLeadCaller.Application.Repositories;

namespace Plat4Me.DialLeadCaller.Infrastructure.Repositories;

public class AgentRepository : IAgentRepository
{
    private readonly DialDbContext _context;

    public AgentRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<User>> GetAll(long clientId, CancellationToken ct = default)
    {
        var entities = await _context.Users
            .Where(r => !r.DeletedAt.HasValue
                        && r.RoleType == RoleTypes.Agent)
            .ToArrayAsync(ct);

        return entities;
    }

    public async Task<User?> GetById(long clientId, long agentId, CancellationToken ct = default)
    {
        var entity = await _context.Users
            .Where(r => r.UserId == agentId)
            .FirstOrDefaultAsync(ct);

        return entity;
    }
}
