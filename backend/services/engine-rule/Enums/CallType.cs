using System.ComponentModel;

namespace KL.Engine.Rule.Enums;

public enum CallType
{
    [Description("Manual calls")]
    Manual = 1,
    [Description("Predictive calls")]
    Predictive = 2,
}
