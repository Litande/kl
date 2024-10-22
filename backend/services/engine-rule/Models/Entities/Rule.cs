using KL.Engine.Rule.Enums;

namespace KL.Engine.Rule.Models.Entities;

public class Rule
{
    public long Id { get; set; }
    public long RuleGroupId { get; set; }
    public string Name { get; set; } = null!;
    public RuleStatusTypes Status { get; set; } = RuleStatusTypes.Enable;
    public string Rules { get; set; } = null!;
    public long? QueueId { get; set; }
    public int Ordinal { get; set; }

    public virtual RuleGroup RuleGroup { get; set; } = null!;
}
