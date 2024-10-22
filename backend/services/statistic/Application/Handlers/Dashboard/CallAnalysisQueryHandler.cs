using KL.Statistics.Application.Common.Enums;
using KL.Statistics.Application.Models.Responses;
using KL.Statistics.Application.Services;

namespace KL.Statistics.Application.Handlers.Dashboard;

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