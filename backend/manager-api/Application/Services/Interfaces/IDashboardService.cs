using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Application.Models.Responses.Dashboard;

namespace Plat4Me.DialClientApi.Application.Services.Interfaces;

public interface IDashboardService
{
    Task<CallAnalysisResponse> CalculateCallAnalysis(long clientId, PeriodTypes periodType, CancellationToken ct = default);
}