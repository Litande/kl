using KL.Agent.API.Application.Enums;

namespace KL.Agent.API.Persistent.Entities;

public class User
{
    public long UserId { get; set; }
    public long ClientId { get; set; }
    public RoleTypes RoleType { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset? OfflineSince { get; set; }
}