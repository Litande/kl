namespace Plat4Me.DialLeadProvider.Application.Models.Requests;

public class LeadCreateRequest
{
    public string? Phone { get; set; }
    public string? ExternalId { get; set; }
    public string? FirstName { get; set; } = null!;
    public string? LastName { get; set; } = null!;
    public string? LanguageCode { get; set; }
    public string? CountryCode { get; set; }
    public string? StatusId { get; set; }
    public DateTimeOffset? LastTimeOnline { get; set; }
    public DateTimeOffset? RegistrationTime { get; set; }
    public long? AssignedUserId { get; set; }
    public bool IsFixedAssigned { get; set; } = false;
    public DateTimeOffset? FirstDepositTime { get; set; }
}