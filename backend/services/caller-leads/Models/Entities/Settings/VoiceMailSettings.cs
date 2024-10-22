using KL.Caller.Leads.Enums;

namespace KL.Caller.Leads.Models.Entities.Settings;

public record VoiceMailSettings(
    LeadStatusTypes PlayVoicemailPlayback,
    LeadStatusTypes VoicemailButtonStatus,
    LeadStatusTypes DefaultVoicemailStatus,
    string HideVoicemailButtonAfterThisAmountOfSecondsOfCall,
    string ShowVoicemailButtonAfterThisAmountOfSecondsOfCall,
    bool HideVoicemailButtonWhenDialingToPermanentlyAssignedLead
    );
