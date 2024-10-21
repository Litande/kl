using Plat4Me.DialRuleEngine.Application.Enums;

namespace Plat4Me.DialRuleEngine.Application.Models.Entities;

public class LeadQueue
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public string Name { get; set; } = null!;
    public LeadQueueStatusTypes Status { get; set; } = LeadQueueStatusTypes.Enable;
    public LeadQueueTypes Type { get; set; } = LeadQueueTypes.Default;
}
