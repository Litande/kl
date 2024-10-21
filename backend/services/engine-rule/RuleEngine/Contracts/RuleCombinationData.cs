using  Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;

public class RuleCombinationData
{
    public RuleCombinationOperator Operator { get; set; }
    public List<RuleGroupData>? Groups { get; set; }
    public List<RuleCombinationData>? Combination { get; set; }
}
