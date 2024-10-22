using KL.Caller.Leads.Enums;

namespace KL.Caller.Leads.Models.Entities;

public class Tag
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public int Value { get; set; }
    public TagStatusTypes Status { get; set; }
    public int? LifetimeSeconds { get; set; }

    public virtual ICollection<UserTag> UserTags { get; set; } = new HashSet<UserTag>();
}
