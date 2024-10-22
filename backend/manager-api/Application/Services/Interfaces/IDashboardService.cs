using KL.Manager.API.Application.Enums;
using KL.Manager.API.Application.Models.Responses.Dashboard;

namespace KL.Manager.API.Application.Services.Interfaces;

public interface IDashboardService
{
    Task<CallAnalysisResponse> CalculateCallAnalysis(long clientId, PeriodTypes periodType, CancellationToken ct = default);
}