namespace KL.Agent.API.Persistent.Entities.Projections;

public class CDRFeedbackTimeoutProjection
{
    public long AgentId { get; set; }
    public long LeadId { get; set; }
    public long ClientId { get; set; }
    public string SessionId { get; set; } = null!;
    public DateTimeOffset CallFinishedAt { get; set; }
}
