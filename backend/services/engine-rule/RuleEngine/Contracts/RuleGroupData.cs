using KL.Engine.Rule.RuleEngine.Enums;

namespace KL.Engine.Rule.RuleEngine.Contracts;

public class RuleGroupData
{
    public string Name { get; set; }
    public ComparisonOperation? ComparisonOperation { get; set; }
    public List<RuleValueData>? Fields { get; set; } = null;
}
