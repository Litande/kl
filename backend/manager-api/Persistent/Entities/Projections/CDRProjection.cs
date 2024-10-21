using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Persistent.Entities.Projections;

public class CDRProjection
{
    public long CallId { get; set; }
    public long? LeadId { get; set; }
    public string? LeadName { get; set; }
    public string LeadPhone { get; set; } = "";
    public LeadStatusTypes? LeadStatusAfter { get; set; }
    public long? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Brand { get; set; }
    public string? GroupName { get; set; }
    public CallType CallType { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public int Duration { get; set; }
    public int BillDuration { get; set; }
    public CallFinishReasons HangupStatus { get; set; }
    public bool HasMixedRecord { get; set; }

}
