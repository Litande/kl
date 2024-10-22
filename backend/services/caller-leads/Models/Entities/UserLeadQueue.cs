namespace KL.Caller.Leads.Models.Entities;

public class UserLeadQueue
{
    public long UserId { get; set; }
    public long LeadQueueId { get; set; }

    public virtual User Agent { get; set; } = null!;
    public virtual LeadQueue LeadQueue { get; set; } = null!;
}
