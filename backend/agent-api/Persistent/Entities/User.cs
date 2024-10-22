using KL.Agent.API.Application.Enums;
using Microsoft.AspNetCore.Identity;

namespace KL.Agent.API.Persistent.Entities;

public class User: IdentityUser<long>
{
    public long ClientId { get; set; }
    public RoleTypes RoleType { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset? OfflineSince { get; set; }
}