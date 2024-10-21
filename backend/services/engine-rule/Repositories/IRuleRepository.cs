using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Models;

namespace Plat4Me.DialRuleEngine.Application.Repositories;

public interface IRuleRepository
{
    Task<IReadOnlyCollection<RuleDto>> GetRulesByType(long clientId, RuleGroupTypes type, CancellationToken ct = default);
}
