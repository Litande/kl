using Plat4Me.Dial.Statistic.Api.Application.Common.Enums;
using Plat4Me.Dial.Statistic.Api.Application.Models.Responses;

namespace Plat4Me.Dial.Statistic.Api.Application.Handlers.Dashboard;

public interface IPerformancePlotQueryHandler
{
    Task<PerformancePlotData> Handle(long clientId, PerformanceTypes type, DateTimeOffset from, DateTimeOffset to, int step, CancellationToken ct = default);
}