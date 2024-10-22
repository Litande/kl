using KL.Provider.Leads.Application.Enums;
using Microsoft.AspNetCore.Identity;

namespace KL.Provider.Leads.Persistent.Entities;

public class User: IdentityUser<long>
{
    public long ClientId { get; set; }
    public RoleTypes RoleType { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}