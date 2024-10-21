using Plat4Me.Dial.Statistic.Api.Application.Common.Enums;
using Plat4Me.Dial.Statistic.Api.Application.Models.Responses;

namespace Plat4Me.Dial.Statistic.Api.Application.Handlers.Dashboard;

public interface IPerformanceStatisticQueryHandler
{
    Task<PerformanceStatisticsData> Handle(long clientId, PerformanceTypes type, DateTimeOffset from, DateTimeOffset to, CancellationToken ct = default);
    Task<IReadOnlyCollection<PerformanceStatisticsData>> Handle(long clientId, PerformanceTypes[] type, DateTimeOffset from, DateTimeOffset to, CancellationToken ct = default);
}