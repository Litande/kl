using System.ComponentModel;

namespace Plat4Me.DialClientApi.Application.Enums;

public enum TimeUnits
{
    [Description("minutes")]
    Minutes = 1,

    [Description("hours")]
    Hours = 2,

    [Description("days")]
    Days = 3,
}