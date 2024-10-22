using KL.Manager.API.Application.Enums;
using Microsoft.AspNetCore.Identity;

namespace KL.Manager.API.Persistent.Entities;

public class User: IdentityUser<long>
{
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
