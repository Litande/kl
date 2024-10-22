using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Models.Responses.Dashboard;

public record PerformancePlotData(
    PerformanceTypes Type,
    DateTimeOffset From,
    DateTimeOffset To,
    long Step,
    PerformancePlotDataItem[] Values
);

public record PerformancePlotDataItem(DateTimeOffset Date, long Value);