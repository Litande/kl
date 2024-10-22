using KL.Statistics.Application.Common.Enums;
using KL.Statistics.Application.Models.Responses;

namespace KL.Statistics.Application.Handlers.Dashboard;

public interface IPerformanceStatisticQueryHandler
{
    Task<PerformanceStatisticsData> Handle(long clientId, PerformanceTypes type, DateTimeOffset from, DateTimeOffset to, CancellationToken ct = default);
    Task<IReadOnlyCollection<PerformanceStatisticsData>> Handle(long clientId, PerformanceTypes[] type, DateTimeOffset from, DateTimeOffset to, CancellationToken ct = default);
}