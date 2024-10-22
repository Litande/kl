namespace KL.Agent.API.Persistent.Entities;

public class LeadComment
{
    public long Id { get; set; }
    public string Comment { get; set; } = null!;
    public long LeadId { get; set; }
    public long CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Lead Lead { get; set; } = null!;
    public User CreatedBy { get; set; } = null!;
}