using KL.Engine.Rule.Enums;
using KL.Engine.Rule.RuleEngine.Contracts;

namespace KL.Engine.Rule.RuleEngine.Conditions;

public interface IRuleCondition
{
    string Key { get; }
    IReadOnlyCollection<RuleGroupTypes> AvailableFor { get; }
    RuleConditionDescription Description { get; }
    Task<Rule> Prepare(RuleGroupData data);
}
