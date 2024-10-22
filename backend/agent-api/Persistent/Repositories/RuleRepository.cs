using KL.Agent.API.Application.Enums;
using KL.Agent.API.Persistent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KL.Agent.API.Persistent.Repositories;

public class RuleRepository : IRuleRepository
{
    private readonly KlDbContext _context;

    public RuleRepository(KlDbContext context)
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
