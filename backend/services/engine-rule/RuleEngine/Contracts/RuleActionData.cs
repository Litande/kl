using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;

public class RuleActionData
{
    public string Name { get; set; }
    public ActionOperation? ActionOperation { get; set; }
    public List<RuleValueData>? Fields { get; set; } = null;
}
