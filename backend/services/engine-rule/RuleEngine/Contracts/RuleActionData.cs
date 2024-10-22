using KL.Engine.Rule.RuleEngine.Enums;

namespace KL.Engine.Rule.RuleEngine.Contracts;

public class RuleActionData
{
    public string Name { get; set; }
    public ActionOperation? ActionOperation { get; set; }
    public List<RuleValueData>? Fields { get; set; } = null;
}
