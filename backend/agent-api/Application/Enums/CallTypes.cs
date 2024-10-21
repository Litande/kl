using System.ComponentModel;

namespace Plat4Me.DialAgentApi.Application.Enums;
public enum CallType
{
    [Description("Manual calls")]
    Manual = 1,
    [Description("Predictive calls")]
    Predictive = 2
}
