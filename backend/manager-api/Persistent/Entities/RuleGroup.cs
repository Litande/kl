using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Persistent.Entities;

public class RuleGroup
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public string Name { get; set; } = null!;
    public StatusTypes Status { get; set; } = StatusTypes.Enable;
    public RuleGroupTypes GroupType { get; set; }

    public virtual ICollection<Rule> Rules { get; set; } = new HashSet<Rule>();
}
