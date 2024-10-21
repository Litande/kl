using Microsoft.AspNetCore.Mvc;
using Plat4Me.DialClientApi.Application.Handlers.LeadGroups;
using Plat4Me.DialClientApi.Application.Handlers.LiveTracking;

namespace Plat4Me.DialClientApi.Controllers;

[Route("tracking")]
public class TrackingController : ApiAuthorizeBase
{
    private readonly IStatusQueryHandler _statusQueryHandler;
    private readonly IAgentsQueryHandler _agentsQueryHandler;

    public TrackingController(
        IStatusQueryHandler statusQueryHandler,
        IAgentsQueryHandler agentsQueryHandler)
    {
        _statusQueryHandler = statusQueryHandler;
        _agentsQueryHandler = agentsQueryHandler;
    }

    [HttpGet]
    [Route("groups")]
    public async Task<IActionResult> LeadGroups(CancellationToken ct)
    {
        var response = await _statusQueryHandler.Handle(CurrentClientId, ct);
        return Ok(response);
    }

    [HttpGet]
    [Route("live_tracking")]
    public async Task<ActionResult> LiveTracking()
    {
        var response = await _agentsQueryHandler.Handle(CurrentClientId);
        return Ok(response);
    }
}
