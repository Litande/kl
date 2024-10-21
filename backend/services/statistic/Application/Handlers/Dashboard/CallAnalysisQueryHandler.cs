using Plat4Me.Dial.Statistic.Api.Application.Common.Enums;
using Plat4Me.Dial.Statistic.Api.Application.Models.Responses;
using Plat4Me.Dial.Statistic.Api.Application.Services;

namespace Plat4Me.Dial.Statistic.Api.Application.Handlers.Dashboard;

public class CallAnalysisQueryHandler : ICallAnalysisQueryHandler
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<CallAnalysisQueryHandler> _logger;
    public CallAnalysisQueryHandler(
        IDashboardService dashboardService,
        ILogger<CallAnalysisQueryHandler> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }
    public async Task<CallAnalysisResponse> Handle(
        long clientId,
        PeriodTypes periodType,
        CancellationToken ct = default)
    {
        _logger.LogInformation("{CallAnalysisQueryHandler} requested with type {periodType} for client Id: {clientId}",
            nameof(CallAnalysisQueryHandler), periodType, clientId);

        var response = await _dashboardService.CalculateCallAnalysis(clientId, periodType, ct);
        return response;
    }
}