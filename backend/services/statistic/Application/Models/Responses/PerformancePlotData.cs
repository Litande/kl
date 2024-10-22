using KL.Statistics.Application.Common.Enums;

namespace KL.Statistics.Application.Models.Responses;

public record PerformancePlotData(
    PerformanceTypes Type,
    DateTimeOffset From,
    DateTimeOffset To,
    long Step,
    PerformancePlotDataItem[] Values
);

public record PerformancePlotDataItem(DateTimeOffset Date, long Value);