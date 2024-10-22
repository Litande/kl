using KL.Caller.Leads.Enums;
using KL.Caller.Leads.Models;
using KL.Caller.Leads.Models.Entities;

namespace KL.Caller.Leads.Repositories;

public class LeadQueueRepository : ILeadQueueRepository
{
    private readonly DialDbContext _context;

    public LeadQueueRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<LeadQueue>> GetAll(CancellationToken ct = default)
    {
        var entities = await _context.LeadQueues
            .AsNoTracking()
            .Where(r => r.Status == LeadQueueStatusTypes.Enable)
            .ToArrayAsync(ct);

        return entities;
    }

    public async Task<IReadOnlyCollection<LeadQueueAgents>> GetAllWithAgentsOrdered(CancellationToken ct = default)
    {
        var entities = await _context.LeadQueues
            .Where(r => r.Status == LeadQueueStatusTypes.Enable)
            .Select(r => new LeadQueueAgents
            {
                Id = r.Id,
                ClientId = r.ClientId,
                Name = r.Name,
                Ratio = r.Ratio,
                Priority = r.Priority,
                QueueType = r.QueueType,
                AssignedAgentIds = r.AgentLeadQueues
                    .Select(p => p.UserId),
            })
            .OrderByDescending(r => r.Priority)
            .ToArrayAsync(ct);

        return entities;
    }

    public async Task<LeadQueueAgents?> GetWithAgents(long queueId, CancellationToken ct = default)
    {
        var entity = await _context.LeadQueues
            .Where(r => r.Status == LeadQueueStatusTypes.Enable
                        && r.Id == queueId)
            .Select(r => new LeadQueueAgents
            {
                Id = r.Id,
                ClientId = r.ClientId,
                Name = r.Name,
                Ratio = r.Ratio,
                Priority = r.Priority,
                QueueType = r.QueueType,
                AssignedAgentIds = r.AgentLeadQueues
                    .Select(p => p.UserId),
            })
            .FirstOrDefaultAsync(ct);

        return entity;
    }

    public async Task UpdateRatio(
        IDictionary<long, double> queueRatioUpdates,
        CancellationToken ct = default)
    {
        var entities = await _context.LeadQueues
            .Where(r => queueRatioUpdates.Keys.Contains(r.Id))
            .ToArrayAsync(ct);

        foreach (var item in entities)
        {
            var ratio = queueRatioUpdates[item.Id];
            if (item.MaxRatio < ratio)
                ratio = item.MaxRatio.Value;
            else if (item.MinRatio > ratio)
                ratio = item.MinRatio.Value;

            item.Ratio = ratio;
            item.RatioUpdatedAt = DateTimeOffset.UtcNow;
        }

        await _context.SaveChangesAsync(ct);
    }
}