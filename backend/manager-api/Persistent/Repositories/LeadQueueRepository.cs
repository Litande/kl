using KL.Manager.API.Application.Enums;
using KL.Manager.API.Application.Extensions;
using KL.Manager.API.Application.Models.Requests.LeadQueue;
using KL.Manager.API.Application.Models.Responses.LeadQueues;
using KL.Manager.API.Persistent.Entities;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KL.Manager.API.Persistent.Repositories;

public class LeadQueueRepository : ILeadQueueRepository
{
    private readonly DialDbContext _context;

    private IQueryable<LeadQueue> ActiveLeadQueues =>
        _context.LeadQueues.Where(x => x.Status == LeadQueueStatusTypes.Enable);

    public LeadQueueRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task<LeadQueuesResponse> GetAll(
        long clientId,
        LeadQueueStatusTypes? status = LeadQueueStatusTypes.Enable,
        CancellationToken ct = default)
    {
        var q = _context.LeadQueues
            .Where(r => r.ClientId == clientId);

        if (status.HasValue)
            q = q.Where(r => r.Status == status);

        var entities = await q
            .AsNoTracking()
            .ToArrayAsync(ct);

        var items = entities.Select(r => r.ToResponse());

        return new LeadQueuesResponse(items);
    }

    public async Task<IReadOnlyCollection<LeadQueue>> GetEnabledQueuesWithAgents(
        long clientId,
        CancellationToken ct = default)
    {
        var leadQueues = await ActiveLeadQueues
            .AsNoTracking()
            .Where(x => x.ClientId == clientId)
            .Include(r => r.Agents)
            .OrderBy(x => x.DisplayOrder)
            .ToArrayAsync(ct);

        return leadQueues;
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

    public async Task<LeadQueue?> GetEnabledQueueByAgent(long clientId, long agentId, CancellationToken ct = default)
    {
        return await ActiveLeadQueues
            .AsNoTracking()
            .Where(x => x.ClientId == clientId
                        && x.Agents.Any(y => y.UserId == agentId))
            .Include(r => r.Agents)
            .FirstOrDefaultAsync(ct);
    }

    public async Task UpdateLeadQueue(
        long clientId,
        long leadQueueId,
        UpdateLeadQueueRequest request,
        CancellationToken ct = default)
    {
        var leadQueue =
            await _context.LeadQueues
                .FirstOrDefaultAsync(x => x.Id == leadQueueId
                                          && x.ClientId == clientId, ct);

        if (leadQueue is null)
            throw new Exception($"LeadQueue with id {leadQueueId} not found");

        leadQueue.DropRateUpperThreshold = request.DropRateUpperThreshold;
        leadQueue.DropRateLowerThreshold = request.DropRateLowerThreshold;
        leadQueue.DropRatePeriod = request.DropRatePeriod;
        leadQueue.RatioStep = request.RatioStep;
        leadQueue.RatioFreezeTime = request.RatioFreezeTime;
        leadQueue.MaxRatio = request.MaxRatio;
        leadQueue.MinRatio = request.MinRatio;

        await _context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyCollection<LeadQueue>> GetEnabledQueues(
        long clientId,
        IReadOnlyCollection<long> queueIds,
        CancellationToken ct = default)
    {
        var q = ActiveLeadQueues
            .AsNoTracking()
            .Where(x => x.ClientId == clientId);

        if (queueIds.Any())
            q = q.Where(x => queueIds.Contains(x.Id));

        var leadQueues = await q
            .Include(r => r.Agents)
            .OrderBy(x => x.DisplayOrder)
            .ToArrayAsync(ct);

        return leadQueues;
    }

    public async Task<LeadQueue?> GetEnabledQueue(long clientId, long queueId, CancellationToken ct = default)
    {
        var q = ActiveLeadQueues
            .AsNoTracking()
            .Where(x => x.ClientId == clientId && x.Id == queueId);

        return await q
            .Include(r => r.Agents)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<LeadQueue?> UpdateRatio(
        long clientId,
        long queueId,
        double ratio,
        CancellationToken ct = default)
    {
        var leadQueue = await ActiveLeadQueues
            .Where(x => x.ClientId == clientId
                        && x.Id == queueId)
            .FirstOrDefaultAsync(ct);

        if (leadQueue is null)
            return null;

        if (leadQueue.QueueType == LeadQueueTypes.Future)
            throw new Exception($"Cannot change ratio in {nameof(LeadQueue)} {queueId}, " +
                                $"queue is a {nameof(LeadQueueTypes.Future)} type");

        if (leadQueue.MaxRatio < ratio || ratio < leadQueue.MinRatio)
        {
            throw new Exception($"Cannot change ratio in {nameof(LeadQueue)} {queueId}, " +
                                $"max ratio is {leadQueue.MaxRatio} and min ratio is {leadQueue.MinRatio}");
        }

        leadQueue.Ratio = ratio;
        leadQueue.RatioUpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(ct);

        return leadQueue;
    }
}
