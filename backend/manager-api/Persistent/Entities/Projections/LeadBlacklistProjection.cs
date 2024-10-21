using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Persistent.Entities.Projections;

public class LeadBlacklistProjection
{
    public long Id { get; set; }
    public string Phone { get; set; } = "";
    public string? Country { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Source { get; set; } = "";
    public string? Email { get; set; }
    public LeadStatusTypes LeadStatus { get; set; }
    public string? Language { get; set; }
    public DateTimeOffset? LastTimeOnline { get; set; }
    public DateTimeOffset RegistrationTime { get; set; }
    public DateTimeOffset? FirstDepositTime { get; set; }
}