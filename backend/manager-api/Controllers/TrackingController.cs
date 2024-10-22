using KL.Manager.API.Application.Handlers.LeadGroups;
using KL.Manager.API.Application.Handlers.LiveTracking;
using Microsoft.AspNetCore.Mvc;

namespace KL.Manager.API.Controllers;

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
