using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Persistent.Entities;

public class StatusRule
{
    public LeadStatusTypes Status { get; set; }
    public LeadStatusTypes AllowTransitStatus { get; set; }
    public long? ClientId { get; set; }

    //public virtual Client Client { get; set; }
}
