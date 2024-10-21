using Plat4Me.DialLeadProvider.Application.Enums;

namespace Plat4Me.DialLeadProvider.Persistent.Entities;

public class LeadHistory
{
    public long Id { get; set; }
    public long LeadId { get; set; }
    public LeadHistoryActionType ActionType { get; set; }
    public string Changes { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public long? CreatedBy { get; set; }
}