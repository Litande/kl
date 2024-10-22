namespace KL.Caller.Leads.Models.Entities;

public class UserTag
{
    public long UserId { get; set; }
    public long TagId { get; set; }
    public DateTimeOffset? ExpiredOn { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Tag Tag { get; set; } = null!;
}
