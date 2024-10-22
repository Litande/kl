using System.ComponentModel;

namespace KL.Engine.Rule.RuleEngine.Enums;

public enum ComparisonOperation
{
    [Description("is")]
    Is,
    [Description("is not")]
    IsNot,

    [Description("equal")]
    Equal,
    [Description("not equal")]
    NotEqual,

    [Description("more than")]
    MoreThan,
    [Description("less than")]
    LessThan,
    [Description("equal or more than")]
    MoreThanEqual,
    [Description("equal or less than")]
    LessThanEqual,

    [Description("equal for last Y hours")]
    EqualForLastYHours,
    [Description("equal for last Y days")]
    EqualForLastYDays,
    [Description("not equal for last Y hours")]
    NotEqualForLastYHours,
    [Description("not equal for last Y days")]
    NotEqualForLastYDays,

    [Description("more than for last Y hours")]
    MoreThanForLastYHours,
    [Description("more than for last Y days")]
    MoreThanForLastYDays,
    [Description("less than for Y last hours")]
    LessThanForLastYHours,
    [Description("less than for Y last days")]
    LessThanForLastYDays,

    [Description("equal or more than for last Y hours")]
    MoreThanEqualForLastYHours,
    [Description("equal or more than for last Y days")]
    MoreThanEqualForLastYDays,
    [Description("equal or less than for Y last hours")]
    LessThanEqualForLastYHours,
    [Description("equal or less than for Y last days")]
    LessThanEqualForLastYDays,

    // [Description("Has value")]
    // HasValue,
    [Description("Contains")]
    Contains,
    [Description("Not contains")]
    NotContains
}
