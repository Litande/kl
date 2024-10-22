using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Extensions;
using KL.Engine.Rule.Models;
using Microsoft.EntityFrameworkCore;

namespace KL.Engine.Rule.Repositories;

public class RuleRepository(KlDbContext context) : IRuleRepository
{
    private IQueryable<Models.Entities.Rule> ActiveRules => context.Rules
        .Where(x => x.Status == RuleStatusTypes.Enable
                    && x.RuleGroup.Status == RuleGroupStatusTypes.Enable);

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
