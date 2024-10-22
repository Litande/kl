namespace KL.Manager.API.Persistent.Entities.Projections;

public class LeadSearchProjection
{
    public long LeadId { get; set; }
    public string PhoneNumber { get; set; } = "";
    public string? Country { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string? BrandName { get; set; }
    public DateTimeOffset RegistrationTime { get; set; }
    public string? Email { get; set; }
    public long LeadScore { get; set; }
    public long TotalCalls { get; set; }
    public string LeadStatus { get; set; } = null!;
    public bool IsBlocked { get; set; }
    public string? Metadata { get; set; }
    public long DataSourceId { get; set; }
    public AssignedAgentProjection? AssignedAgent { get; set; }
}