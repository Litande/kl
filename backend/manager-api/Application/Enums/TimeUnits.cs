using System.ComponentModel;

namespace KL.Manager.API.Application.Enums;

public enum TimeUnits
{
    [Description("minutes")]
    Minutes = 1,

    [Description("hours")]
    Hours = 2,

    [Description("days")]
    Days = 3,
}