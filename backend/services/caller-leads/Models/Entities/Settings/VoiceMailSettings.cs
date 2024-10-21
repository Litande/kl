using Plat4Me.DialLeadCaller.Application.Enums;

namespace Plat4Me.DialLeadCaller.Application.Models.Entities.Settings;

public record VoiceMailSettings(
    LeadStatusTypes PlayVoicemailPlayback,
    LeadStatusTypes VoicemailButtonStatus,
    LeadStatusTypes DefaultVoicemailStatus,
    string HideVoicemailButtonAfterThisAmountOfSecondsOfCall,
    string ShowVoicemailButtonAfterThisAmountOfSecondsOfCall,
    bool HideVoicemailButtonWhenDialingToPermanentlyAssignedLead
    );
