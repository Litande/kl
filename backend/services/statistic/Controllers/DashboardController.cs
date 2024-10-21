using Microsoft.AspNetCore.Mvc;
using Plat4Me.Dial.Statistic.Api.Application.Common.Enums;
using Plat4Me.Dial.Statistic.Api.Application.Handlers.Dashboard;

namespace Plat4Me.Dial.Statistic.Api.Controllers;

[Route("dashboard")]
public class DashboardController : ApiAuthorizeBase
{
    private readonly ICallAnalysisQueryHandler _callAnalysisQueryHandler;
    private readonly IGetAgentsWorkModeHandler _getAgentsWorkModeHandler;
    private readonly IPerformancePlotQueryHandler _performancePlotQueryHandler;
    private readonly IPerformanceStatisticQueryHandler _performanceStatisticQueryHandler;

    public DashboardController(
        ICallAnalysisQueryHandler callAnalysisQueryHandler,
        IGetAgentsWorkModeHandler getAgentsWorkModeHandler,
        IPerformancePlotQueryHandler performancePlotQueryHandler,
        IPerformanceStatisticQueryHandler performanceStatisticQueryHandler)
    {
        _callAnalysisQueryHandler = callAnalysisQueryHandler;
        _getAgentsWorkModeHandler = getAgentsWorkModeHandler;
        _performancePlotQueryHandler = performancePlotQueryHandler;
        _performanceStatisticQueryHandler = performanceStatisticQueryHandler;
    }

    [HttpGet]
    [Route("call_analysis/{periodType}")]
    public async Task<IActionResult> LeadGroups(
        [FromRoute] PeriodTypes periodType,
        CancellationToken ct)
    {
        var response = await _callAnalysisQueryHandler.Handle(CurrentClientId, periodType, ct);
        return Ok(response);
    }

    [HttpGet]
    [Route("agents_work_mode")]
    public async Task<IActionResult> GetAgentsWorkMode(CancellationToken ct)
    {
        var response = await _getAgentsWorkModeHandler.Handle(CurrentClientId, ct);
        return Ok(response);
    }

    [HttpGet("performance/{type}/statistics")]
    public async Task<IActionResult> PerformanceStatistics(
        [FromRoute] PerformanceTypes type,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        CancellationToken ct)
    {
        var response = await _performanceStatisticQueryHandler
            .Handle(CurrentClientId, type, from, to, ct);
        return Ok(response);
    }

    [HttpGet("performance/{type}/plots")]
    public async Task<IActionResult> PerformancePlots(
        [FromRoute] PerformanceTypes type,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        int step,
        CancellationToken ct = default)
    {
        var response = await _performancePlotQueryHandler
            .Handle(CurrentClientId, type, from, to, step, ct);
        return Ok(response);
    }
}