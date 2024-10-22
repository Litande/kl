using KL.Agent.API.Application.Enums;

namespace KL.Agent.API.Persistent.Entities;

public class StatusRule
{
    public LeadStatusTypes Status { get; set; }
    public LeadStatusTypes AllowTransitStatus { get; set; }
    public long? ClientId { get; set; }

    //public virtual Client Client { get; set; }
}
