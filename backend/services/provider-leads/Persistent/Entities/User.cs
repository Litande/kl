using KL.Provider.Leads.Application.Enums;

namespace KL.Provider.Leads.Persistent.Entities;

public class User
{
    public long UserId { get; set; }
    public long ClientId { get; set; }
    public RoleTypes RoleType { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}