using Microsoft.EntityFrameworkCore;
using Plat4Me.DialRuleEngine.Application.Common.Extensions;
using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.Models.Entities;
using Plat4Me.DialRuleEngine.Application.Repositories;

namespace Plat4Me.DialRuleEngine.Infrastructure.Repositories;

public class RuleRepository : IRuleRepository
{
    private readonly DialDbContext _context;

    private IQueryable<Rule> ActiveRules => _context.Rules
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
