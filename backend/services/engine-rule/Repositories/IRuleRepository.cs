using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Models;

namespace KL.Engine.Rule.Repositories;

public interface IRuleRepository
{
    Task<IReadOnlyCollection<RuleDto>> GetRulesByType(long clientId, RuleGroupTypes type, CancellationToken ct = default);
}
