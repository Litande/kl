using KL.Engine.Rule.Enums;

namespace KL.Engine.Rule.Models.Entities;

public class LeadQueue
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public string Name { get; set; } = null!;
    public LeadQueueStatusTypes Status { get; set; } = LeadQueueStatusTypes.Enable;
    public LeadQueueTypes Type { get; set; } = LeadQueueTypes.Default;
}
