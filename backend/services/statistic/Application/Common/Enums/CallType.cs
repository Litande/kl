using System.ComponentModel;

namespace KL.Statistics.Application.Common.Enums;

public enum CallType
{
    [Description("Manual calls")]
    Manual = 1,
    [Description("Predictive calls")]
    Predictive = 2
}
