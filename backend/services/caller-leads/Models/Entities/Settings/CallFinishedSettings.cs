using Plat4Me.DialLeadCaller.Application.Enums;

namespace Plat4Me.DialLeadCaller.Application.Models.Entities.Settings;

public class CallFinishedSettings
{
    public LeadStatusTypes LeadInvalidPhone { get; set; }
    public LeadStatusTypes NoAvailableAgents { get; set; }
    public LeadStatusTypes LeadNotAnswered { get; set; }
    public LeadStatusTypes SystemIssues { get; set; }
    public LeadStatusTypes Default { get; set; }
}
