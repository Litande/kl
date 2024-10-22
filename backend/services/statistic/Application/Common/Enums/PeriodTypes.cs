using System.ComponentModel;

namespace KL.Statistics.Application.Common.Enums;

public enum PeriodTypes
{
    [Description("Today")] Today = 1,
    [Description("Week")] Week = 7,
    [Description("Month")] Month = 30,
    [Description("Year")] Year = 365,
}