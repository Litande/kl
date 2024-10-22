using KL.Statistics.Application.Common.Enums;
using Microsoft.AspNetCore.Identity;

namespace KL.Statistics.Application.Models.Entities;

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
}
