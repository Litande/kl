using System.ComponentModel;

namespace Plat4Me.DialLeadCaller.Application.Enums;

public enum CallStatusType
{
    [Description("Initiated")]
    Initiated = 1,
    [Description("Dialing")]
    Dialing = 2,
    [Description("In Progress")]
    InProgress = 3,
    [Description("Finished")]
    Finished = 4,
    [Description("Failed")]
    Failed = 5,
}
