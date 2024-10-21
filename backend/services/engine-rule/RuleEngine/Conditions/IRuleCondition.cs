using RulesEngine.Models;
using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Conditions;

public interface IRuleCondition
{
    string Key { get; }
    IReadOnlyCollection<RuleGroupTypes> AvailableFor { get; }
    RuleConditionDescription Description { get; }
    Task<Rule> Prepare(RuleGroupData data);
}
