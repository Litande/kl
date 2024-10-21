using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Persistent.Entities;

public class Rule
{
    public long Id { get; set; }
    public long RuleGroupId { get; set; }
    public string Name { get; set; } = null!;
    public StatusTypes Status { get; set; } = StatusTypes.Enable;
    public string Rules { get; set; } = null!;
    public int Ordinal { get; set; }

    public virtual RuleGroup GroupRule { get; set; } = null!;
}