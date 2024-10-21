using Plat4Me.Dial.Statistic.Api.Application.Common.Enums;

namespace Plat4Me.Dial.Statistic.Api.Application.Models.Responses;

public record PerformanceStatisticsData(
    PerformanceTypes Type,
    DateTimeOffset From,
    DateTimeOffset To,
    long Value,
    long Trend
);