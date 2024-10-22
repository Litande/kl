using KL.Caller.Leads.Enums;

namespace KL.Caller.Leads.Models.Entities.Settings;

public class CallFinishedSettings
{
    public LeadStatusTypes LeadInvalidPhone { get; set; }
    public LeadStatusTypes NoAvailableAgents { get; set; }
    public LeadStatusTypes LeadNotAnswered { get; set; }
    public LeadStatusTypes SystemIssues { get; set; }
    public LeadStatusTypes Default { get; set; }
}
