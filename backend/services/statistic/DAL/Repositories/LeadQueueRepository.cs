using KL.Statistics.Application.Common.Enums;
using KL.Statistics.Application.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KL.Statistics.DAL.Repositories;

public class LeadQueueRepository : ILeadQueueRepository
{
    private readonly DialDbContext _context;

    private IQueryable<LeadQueue> ActiveLeadQueues =>
        _context.LeadQueues.Where(x => x.Status == LeadQueueStatusTypes.Enable);

    public LeadQueueRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<LeadQueue>> GetEnabledQueuesByAgents(
        long clientId,
        IEnumerable<long> agentIds,
        CancellationToken ct = default)
    {
        var leadQueues = await ActiveLeadQueues
            .AsNoTracking()
            .Where(x => x.ClientId == clientId
                        && x.Agents.Any(y => agentIds.Contains(y.UserId)))
            .Include(r => r.Agents)
            .OrderBy(x => x.DisplayOrder)
            .ToArrayAsync(ct);

        return leadQueues;
    }
}