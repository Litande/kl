using Plat4Me.Dial.Statistic.Api.Application.Common.Enums;
using Plat4Me.Dial.Statistic.Api.Application.Models.Responses;

namespace Plat4Me.Dial.Statistic.Api.Application.Services;

public interface IDashboardService
{
    Task<CallAnalysisResponse> CalculateCallAnalysis(long clientId, PeriodTypes periodType, CancellationToken ct = default);
}