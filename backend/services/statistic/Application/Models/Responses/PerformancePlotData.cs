using Plat4Me.Dial.Statistic.Api.Application.Common.Enums;

namespace Plat4Me.Dial.Statistic.Api.Application.Models.Responses;

public record PerformancePlotData(
    PerformanceTypes Type,
    DateTimeOffset From,
    DateTimeOffset To,
    long Step,
    PerformancePlotDataItem[] Values
);

public record PerformancePlotDataItem(DateTimeOffset Date, long Value);