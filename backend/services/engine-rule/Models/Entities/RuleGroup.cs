using KL.Engine.Rule.Enums;

namespace KL.Engine.Rule.Models.Entities;

public class RuleGroup
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public string Name { get; set; } = null!;
    public RuleGroupStatusTypes Status { get; set; } = RuleGroupStatusTypes.Enable;
    public RuleGroupTypes GroupType { get; set; }

    public virtual ICollection<Rule> Rules { get; set; } = new HashSet<Rule>();
}
