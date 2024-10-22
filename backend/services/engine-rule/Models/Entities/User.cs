using KL.Engine.Rule.Enums;

namespace KL.Engine.Rule.Models.Entities;

public class User
{
    public long UserId { get; set; }
    public long ClientId { get; set; }
    public RoleTypes RoleType { get; set; } = RoleTypes.Agent;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTimeOffset? OfflineSince { get; set; }
}