using Plat4Me.DialAgentApi.Application.Enums;
using Plat4Me.DialAgentApi.Persistent.Entities;
using Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialAgentApi.Persistent.Repositories;

public class AgentStatusHistoryRepository : IAgentStatusHistoryRepository
{
    private readonly DialDbContext _context;

    public AgentStatusHistoryRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task AddStatusHistory(AgentStatusHistory statusHistory, CancellationToken ct = default)
    {
        await _context.AgentStatusHistories.AddAsync(statusHistory, ct);
        await _context.SaveChangesAsync(ct);
    }
}
