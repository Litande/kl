using Plat4Me.DialRuleEngine.Application.Enums;

namespace Plat4Me.DialRuleEngine.Application.Models.Entities;

public class RuleGroup
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public string Name { get; set; } = null!;
    public RuleGroupStatusTypes Status { get; set; } = RuleGroupStatusTypes.Enable;
    public RuleGroupTypes GroupType { get; set; }

    public virtual ICollection<Rule> Rules { get; set; } = new HashSet<Rule>();
}
