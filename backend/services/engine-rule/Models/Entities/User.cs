using KL.Engine.Rule.Enums;
using Microsoft.AspNetCore.Identity;

namespace KL.Engine.Rule.Models.Entities;

public class User: IdentityUser<long>
{
    public long ClientId { get; set; }
    public RoleTypes RoleType { get; set; } = RoleTypes.Agent;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTimeOffset? OfflineSince { get; set; }
}