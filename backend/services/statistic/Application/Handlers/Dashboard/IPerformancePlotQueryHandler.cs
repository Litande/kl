using KL.Statistics.Application.Common.Enums;
using KL.Statistics.Application.Models.Responses;

namespace KL.Statistics.Application.Handlers.Dashboard;

public interface IPerformancePlotQueryHandler
{
    Task<PerformancePlotData> Handle(long clientId, PerformanceTypes type, DateTimeOffset from, DateTimeOffset to, int step, CancellationToken ct = default);
}