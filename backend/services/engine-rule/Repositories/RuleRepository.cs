using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Models;

namespace KL.Engine.Rule.Repositories;

public class RuleRepository : IRuleRepository
{
    private readonly DialDbContext _context;

    private IQueryable<Models.Entities.Rule> ActiveRules => _context.Rules
        .Where(x => x.Status == RuleStatusTypes.Enable
                    && x.RuleGroup.Status == RuleGroupStatusTypes.Enable);

    public RuleRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<RuleDto>> GetRulesByType(
        long clientId,
        RuleGroupTypes type,
        CancellationToken ct = default)
    {
        return await ActiveRules
            .AsNoTracking()
            .Where(x => x.RuleGroup.ClientId == clientId
                        && x.RuleGroup.GroupType == type)
            .Select(x => x.ToResponse())
            .ToArrayAsync(ct);
    }
}
