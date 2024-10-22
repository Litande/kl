using System.ComponentModel;

namespace KL.Engine.Rule.RuleEngine.Enums;

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
    [Description("integerSet")]
    IntegerSet = 5,
    [Description("stringSet")]
    StringSet = 6,
}
