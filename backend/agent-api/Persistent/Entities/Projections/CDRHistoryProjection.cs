namespace KL.Agent.API.Persistent.Entities.Projections;

public class CDRHistoryProjection
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Country { get; set; }
    public string LeadPhone { get; set; }
    public DateTimeOffset? LastActivity { get; set; }
}
