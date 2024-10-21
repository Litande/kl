using System.ComponentModel;

namespace Plat4Me.DialRuleEngine.Application.Enums;

public enum ConditionsCategory
{
    [Description("Custom Field")]
    CustomField = 1,
    [Description("Lead Field")]
    LeadField = 2,
}
