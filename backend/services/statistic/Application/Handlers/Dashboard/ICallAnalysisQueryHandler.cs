using Plat4Me.Dial.Statistic.Api.Application.Common.Enums;
using Plat4Me.Dial.Statistic.Api.Application.Models.Responses;

namespace Plat4Me.Dial.Statistic.Api.Application.Handlers.Dashboard;

public interface ICallAnalysisQueryHandler
{
    Task<CallAnalysisResponse> Handle(long clientId, PeriodTypes periodType, CancellationToken ct = default);
}