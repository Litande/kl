using KL.Caller.Leads.Enums;
using KL.Caller.Leads.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KL.Caller.Leads.Repositories;

public class AgentRepository(KlDbContext context) : IAgentRepository
{
    public async Task<IReadOnlyCollection<User>> GetAll(long clientId, CancellationToken ct = default)
    {
        var entities = await context.Users
            .Where(r => !r.DeletedAt.HasValue
                        && r.RoleType == RoleTypes.Agent)
            .ToArrayAsync(ct);

        return entities;
    }

    public async Task<User?> GetById(long clientId, long agentId, CancellationToken ct = default)
    {
        var entity = await context.Users
            .Where(r => r.Id == agentId)
            .FirstOrDefaultAsync(ct);

        return entity;
    }
}
