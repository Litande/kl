using KL.Engine.Rule.Enums;
using KL.Engine.Rule.RuleEngine.Contracts;

namespace KL.Engine.Rule.Services.Contracts;

public interface ILeadQueueRuleService
{
    Task ValidateRules(long clientId, RuleGroupTypes ruleType, string rules);
    Task<IEnumerable<RuleActionDescription>> GetActions(long clientId, RuleGroupTypes ruleType, CancellationToken ct = default);
    Task<IEnumerable<RuleConditionDescription>> GetConditions(long clientId, RuleGroupTypes ruleType);
}
