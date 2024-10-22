using System.ComponentModel;

namespace KL.Manager.API.Application.Enums;

public enum CallType
{
    [Description("Manual calls")]
    Manual = 1,
    [Description("Predictive calls")]
    Predictive = 2
}
