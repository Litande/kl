using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Application.Models.Responses.Dashboard;

public record PerformancePlotData(
    PerformanceTypes Type,
    DateTimeOffset From,
    DateTimeOffset To,
    long Step,
    PerformancePlotDataItem[] Values
);

public record PerformancePlotDataItem(DateTimeOffset Date, long Value);