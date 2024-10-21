using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Application.Models.Responses.Dashboard;

public record PerformanceStatisticsData(
    PerformanceTypes Type,
    DateTimeOffset From,
    DateTimeOffset To,
    long Value,
    long Trend
);