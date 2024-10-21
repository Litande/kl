using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;

public class RuleValueData
{
    public RuleValueType Type { get; set; }
    public string? Value { get; set; }
}
