using Plat4Me.DialAgentApi.Application.Enums;

namespace Plat4Me.DialAgentApi.Persistent.Entities.Projections;

public class FutureCallBackProjection
{
    public long LeadId { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? CountryCode { get; set; }
    public string? Email { get; set; }
    public DateTimeOffset RegisteredAt { get; set; }
    public DateTimeOffset? NextCallAt { get; set; }
    public DateTimeOffset? LastCallAt { get; set; }
    public long TotalCallsSecond { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public LeadStatusTypes LeadStatus { get; set; }
    public string Campaign { get; set; } = string.Empty;
    public long? Weight { get; set; }
}
