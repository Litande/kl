using Microsoft.AspNetCore.Mvc;
using Plat4Me.DialAgentApi.Application.Models.Requests;
using Plat4Me.DialAgentApi.Application.Handlers;

namespace Plat4Me.DialAgentApi.Controllers;

[Route("user")]
public class AgentAccountController : ApiAuthorizeBase
{
    private readonly IUserQueryHandler _userQueryHandler;
    private readonly IAgentChangePasswordHandler _agentChangePasswordHandler;

    public AgentAccountController(
        IUserQueryHandler userQueryHandler,
        IAgentChangePasswordHandler agentChangePasswordHandler
    )
    {
        _userQueryHandler = userQueryHandler;
        _agentChangePasswordHandler = agentChangePasswordHandler;
    }

    [HttpGet("me", Order = -1)]
    public async Task<IActionResult> Me()
    {
        var user = await _userQueryHandler.Handle(CurrentClientId, CurrentUserId);
        if (user is null) return NotFound();

        return Ok(user);
    }

    [HttpPost]
    [Route("changepassword")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] AgentChangePasswordRequest changePasswordRequest,
        CancellationToken ct = default)
    {
        await _agentChangePasswordHandler.Handle(CurrentUserId, changePasswordRequest, ct);
        return Ok();
    }
}
