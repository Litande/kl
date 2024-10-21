using System.ComponentModel;

namespace Plat4Me.DialClientApi.Application.Enums;

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

}
