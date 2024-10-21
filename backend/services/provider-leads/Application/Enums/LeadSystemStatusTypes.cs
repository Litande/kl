using System.ComponentModel;

namespace Plat4Me.DialLeadProvider.Application.Enums;

public enum LeadSystemStatusTypes
{
    [Description("Processing")]
    Processing = 1,
    [Description("Dialing")]
    Dialing = 2,
    [Description("In The Call")]
    InTheCall = 3,
    [Description("Waiting Feedback")]
    WaitingFeedback = 4,
    [Description("Post Processing")]
    PostProcessing = 5,
    [Description("Imported")]
    Imported = 6,
}