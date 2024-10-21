using System.ComponentModel;

namespace Plat4Me.DialClientApi.Application.Enums;

public enum RuleValueType
{
    [Description("string")]
    String = 1,
    [Description("integer")]
    Integer = 2,
    [Description("select")]
    Select = 3,
    [Description("selectMultiItem")]
    SelectMultiItem = 4,
}
