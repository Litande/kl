using KL.Agent.API.Persistent.Entities;
using KL.Agent.API.Persistent.Repositories.Interfaces;

namespace KL.Agent.API.Persistent.Repositories;

public class AgentStatusHistoryRepository : IAgentStatusHistoryRepository
{
    private readonly KlDbContext _context;

    public AgentStatusHistoryRepository(KlDbContext context)
    {
        _context = context;
    }

    public async Task AddStatusHistory(AgentStatusHistory statusHistory, CancellationToken ct = default)
    {
        await _context.AgentStatusHistories.AddAsync(statusHistory, ct);
        await _context.SaveChangesAsync(ct);
    }
}
