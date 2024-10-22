namespace KL.Engine.Rule.RuleEngine.Contracts;

public class RuleEntry
{
    public RuleCombinationData Combination { get; set; } = null!;
    public List<RuleActionData> Actions { get; set; } = null!;
}