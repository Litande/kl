using KL.Statistics.Application.Common.Enums;
using KL.Statistics.Application.Models.Responses;

namespace KL.Statistics.Application.Handlers.Dashboard;

public interface ICallAnalysisQueryHandler
{
    Task<CallAnalysisResponse> Handle(long clientId, PeriodTypes periodType, CancellationToken ct = default);
}