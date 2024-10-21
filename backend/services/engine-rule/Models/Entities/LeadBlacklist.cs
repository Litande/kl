namespace Plat4Me.DialRuleEngine.Application.Models.Entities;

public class LeadBlacklist
{
    public long Id { get; set; }
    public long LeadId { get; set; }
    public long CreatedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    
    public virtual Lead Lead { get; set; }
    public virtual User CreatedBy { get; set; }
}