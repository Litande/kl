namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;

public class RuleEntry
{
    public RuleCombinationData Combination { get; set; } = null!;
    public List<RuleActionData> Actions { get; set; } = null!;
}