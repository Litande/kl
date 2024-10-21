using System.ComponentModel;

namespace Plat4Me.Dial.Statistic.Api.Application.Common.Enums;

public enum CallType
{
    [Description("Manual calls")]
    Manual = 1,
    [Description("Predictive calls")]
    Predictive = 2
}
