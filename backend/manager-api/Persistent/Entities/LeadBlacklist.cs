namespace Plat4Me.DialClientApi.Persistent.Entities;

public class LeadBlacklist
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public long LeadId { get; set; }
    public long CreatedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public virtual Client Client { get; set; }
    public virtual Lead Lead { get; set; }
    public virtual User CreatedBy { get; set; }
}
