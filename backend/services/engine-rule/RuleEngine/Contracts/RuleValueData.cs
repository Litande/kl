using KL.Engine.Rule.RuleEngine.Enums;

namespace KL.Engine.Rule.RuleEngine.Contracts;

public class RuleValueData
{
    public RuleValueType Type { get; set; }
    public string? Value { get; set; }
}
