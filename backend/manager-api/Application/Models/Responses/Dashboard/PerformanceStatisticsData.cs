using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Models.Responses.Dashboard;

public record PerformanceStatisticsData(
    PerformanceTypes Type,
    DateTimeOffset From,
    DateTimeOffset To,
    long Value,
    long Trend
);