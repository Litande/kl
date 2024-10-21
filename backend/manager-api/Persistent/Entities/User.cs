using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Persistent.Entities;

public class User
{
    public long UserId { get; set; }
    public long ClientId { get; set; }
    public RoleTypes RoleType { get; set; } = RoleTypes.Agent;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public UserStatusTypes Status { get; set; } = UserStatusTypes.Active;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? Timezone { get; set; }
    public virtual ICollection<LeadQueue> LeadQueues { get; set; } = new HashSet<LeadQueue>();
    public virtual ICollection<UserTag> UserTags { get; set; } = new HashSet<UserTag>();
    public virtual ICollection<Team> Teams { get; set; } = new HashSet<Team>();
}
