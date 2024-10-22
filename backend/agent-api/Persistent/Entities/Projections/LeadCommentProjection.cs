namespace KL.Agent.API.Persistent.Entities.Projections;

public class LeadCommentProjection
{
    public long Id { get; set; }
    public long AgentId { get; set; }
    public string AgentFullName { get; set; } = null!;
    public string Comment { get; set; } = null!;
    public string LeadStatus { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
}