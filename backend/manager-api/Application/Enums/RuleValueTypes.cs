using System.ComponentModel;

namespace KL.Manager.API.Application.Enums;

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
