using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;

public class RuleGroupData
{
    public string Name { get; set; }
    public ComparisonOperation? ComparisonOperation { get; set; }
    public List<RuleValueData>? Fields { get; set; } = null;
}
