using Microsoft.EntityFrameworkCore;
using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Models.Entities;
using Plat4Me.DialRuleEngine.Application.Repositories;

namespace Plat4Me.DialRuleEngine.Infrastructure.Repositories;

public class LeadQueueRepository : ILeadQueueRepository
{
    private readonly IDbContextFactory<DialDbContext> _dbContextFactory;

    public LeadQueueRepository(
        IDbContextFactory<DialDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public IReadOnlyCollection<LeadQueue> GetAll()
    {
        var context = _dbContextFactory.CreateDbContext();
        var entities = context.LeadQueues
            .AsNoTracking()
            .Where(r => r.Status == LeadQueueStatusTypes.Enable)
            .ToArray();

        return entities;
    }

    public async Task<IReadOnlyCollection<LeadQueue>> GetAllByClient(
        long clientId,
        LeadQueueStatusTypes? status = LeadQueueStatusTypes.Enable,
        CancellationToken ct = default)
    {
        var context = await _dbContextFactory.CreateDbContextAsync(ct);
        var query = context.LeadQueues
            .AsNoTracking()
            .Where(r => r.ClientId == clientId);

        if (status.HasValue)
            query = query.Where(r => r.Status == status);

        var entities = await query
            .ToArrayAsync(ct);

        return entities;
    }
}