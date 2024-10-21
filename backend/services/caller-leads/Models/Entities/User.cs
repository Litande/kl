using Plat4Me.DialLeadCaller.Application.Enums;

namespace Plat4Me.DialLeadCaller.Application.Models.Entities;

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

    public virtual ICollection<UserTag> UserTags { get; set; } = new HashSet<UserTag>();
}
