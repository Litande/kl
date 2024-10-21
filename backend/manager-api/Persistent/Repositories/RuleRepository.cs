using Microsoft.EntityFrameworkCore;
using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Persistent.Entities;
using Plat4Me.DialClientApi.Persistent.Entities.Projections;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialClientApi.Persistent.Repositories;

public class RuleRepository : IRuleRepository
{
    private readonly DialDbContext _context;

    public RuleRepository(DialDbContext context)
    {
        _context = context;
    }

    public Task<Rule?> GetById(
        long clientId,
        long ruleId,
        CancellationToken ct = default)
    {
        return _context.Rules
            .Where(x => x.GroupRule.ClientId == clientId
                        && x.Id == ruleId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Rule> AddRule(
        Rule rule,
        CancellationToken ct = default)
    {
        await _context.Rules.AddAsync(rule, ct);
        await _context.SaveChangesAsync(ct);

        return rule;
    }

    public async Task SaveChanges(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<StatusRuleProjection>> GetStatusRules(
        long currentClientId,
        CancellationToken ct = default)
    {
        var statusRules = await _context.StatusRules
            .Where(x => x.ClientId == currentClientId)
            .GroupBy(x => x.Status)
            .Select(
                x => new StatusRuleProjection
                {
                    Status = x.Key.ToString(),
                    AvailableStatuses = x.Select(r => r.AllowTransitStatus.ToString())
                })
            .ToArrayAsync(ct);

        return statusRules;
    }

    public async Task UpdateStatusRules(
        long currentClientId,
        IEnumerable<StatusRuleProjection> rules,
        CancellationToken ct = default
    )
    {
        var newRuleSet = rules.SelectMany(x => x.AvailableStatuses, (x, r) => new StatusRule
        {
            Status = Enum.Parse<LeadStatusTypes>(x.Status, true),
            AllowTransitStatus = Enum.Parse<LeadStatusTypes>(r, true),
            ClientId = currentClientId
        });

        var statusRules = await _context.StatusRules.Where(x => x.ClientId == currentClientId).ToArrayAsync(ct);
        _context.StatusRules.RemoveRange(statusRules);
        await _context.StatusRules.AddRangeAsync(newRuleSet, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> Delete(
        long ruleId,
        CancellationToken ct = default)
    {
        var entity = await _context.Rules
            .Where(r => r.Id == ruleId)
            .FirstOrDefaultAsync(ct);

        if (entity is null) return false;

        _context.Rules.Remove(entity);
        await _context.SaveChangesAsync(ct);

        return true;
    }
}
