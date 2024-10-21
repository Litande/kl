using Microsoft.EntityFrameworkCore;
using Plat4Me.DialAgentApi.Application.Enums;
using Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialAgentApi.Persistent.Repositories;

public class RuleRepository : IRuleRepository
{
    private readonly DialDbContext _context;

    public RuleRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<LeadStatusTypes>> GetAvailableStatuses(
        long clientId,
        LeadStatusTypes currentStatus,
        CancellationToken ct = default)
    {
        var statusRules = await _context.StatusRules
            .Where(x => x.ClientId == clientId
                        && x.Status == currentStatus)
            .Select(x => x.AllowTransitStatus)
            .ToArrayAsync(ct);
        return statusRules;
    }
}
