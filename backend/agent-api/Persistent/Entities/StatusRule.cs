using Plat4Me.DialAgentApi.Application.Enums;

namespace Plat4Me.DialAgentApi.Persistent.Entities;

public class StatusRule
{
    public LeadStatusTypes Status { get; set; }
    public LeadStatusTypes AllowTransitStatus { get; set; }
    public long? ClientId { get; set; }

    //public virtual Client Client { get; set; }
}
