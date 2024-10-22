using KL.Statistics.Application.Common.Enums;
using KL.Statistics.Application.Models.Responses;

namespace KL.Statistics.Application.Services;

public interface IDashboardService
{
    Task<CallAnalysisResponse> CalculateCallAnalysis(long clientId, PeriodTypes periodType, CancellationToken ct = default);
}