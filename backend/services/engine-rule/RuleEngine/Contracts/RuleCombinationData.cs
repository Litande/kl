using KL.Engine.Rule.RuleEngine.Enums;

namespace KL.Engine.Rule.RuleEngine.Contracts;

public class RuleCombinationData
{
    public RuleCombinationOperator Operator { get; set; }
    public List<RuleGroupData>? Groups { get; set; }
    public List<RuleCombinationData>? Combination { get; set; }
}
