using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;

namespace Plat4Me.DialRuleEngine.Application.Services.Contracts;

public interface ILeadQueueRuleService
{
    Task ValidateRules(long clientId, RuleGroupTypes ruleType, string rules);
    Task<IEnumerable<RuleActionDescription>> GetActions(long clientId, RuleGroupTypes ruleType, CancellationToken ct = default);
    Task<IEnumerable<RuleConditionDescription>> GetConditions(long clientId, RuleGroupTypes ruleType);
}
