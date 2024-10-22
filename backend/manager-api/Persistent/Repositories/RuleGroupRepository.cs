using KL.Manager.API.Application.Enums;
using KL.Manager.API.Persistent.Entities;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KL.Manager.API.Persistent.Repositories;

public class RuleGroupRepository : IRuleGroupRepository
{
    private readonly KlDbContext _context;

    public RuleGroupRepository(KlDbContext context)
    {
        _context = context;
    }

    public Task<RuleGroup?> GetById(
        long currentClientId,
        long ruleGroupId,
        CancellationToken ct = default)
    {
        return _context.GroupRules
            .Where(i => i.ClientId == currentClientId
                        && i.Id == ruleGroupId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<RuleGroup>> GetRuleGroups(
        long currentClientId,
        RuleGroupTypes ruleType,
        CancellationToken ct = default)
    {
        var ruleGroups = await _context.GroupRules
            .Where(i => i.ClientId == currentClientId
                        && i.GroupType == ruleType)
            .Include(i => i.Rules.OrderBy(x => x.Ordinal))
            .ToArrayAsync(ct);

        return ruleGroups;
    }

    public async Task<RuleGroup> AddRuleGroup(
        RuleGroup ruleGroup,
        CancellationToken ct = default)
    {
        await _context.GroupRules.AddAsync(ruleGroup, ct);
        await _context.SaveChangesAsync(ct);

        return ruleGroup;
    }

    public async Task<bool> Delete(
        long groupId,
        CancellationToken ct = default)
    {
        var entity = await _context.GroupRules
            .Where(r => r.Id == groupId)
            .Include(r => r.Rules)
            .FirstOrDefaultAsync(ct);

        if (entity is null) return false;

        foreach (var item in entity.Rules)
        {
            entity.Rules.Remove(item);
        }

        _context.GroupRules.Remove(entity);
        await _context.SaveChangesAsync(ct);

        return true;
    }

    public async Task SaveChanges(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}