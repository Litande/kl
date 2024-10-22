using KL.Statistics.Application.Common.Enums;

namespace KL.Statistics.Application.Models.Responses;

public record PerformanceStatisticsData(
    PerformanceTypes Type,
    DateTimeOffset From,
    DateTimeOffset To,
    long Value,
    long Trend
);