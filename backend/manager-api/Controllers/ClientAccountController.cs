using Microsoft.AspNetCore.Mvc;
using Plat4Me.DialClientApi.Application.Handlers;

namespace Plat4Me.DialClientApi.Controllers;

[Route("user")]
public class ClientAccountController : ApiAuthorizeBase
{

    private readonly IUserQueryHandler _userQueryHandler;

    public ClientAccountController(IUserQueryHandler userQueryHandler)
    {
        _userQueryHandler = userQueryHandler;
    }

    [HttpGet("me", Order = -1)]
    public async Task<IActionResult> Me()
    {
        var user = await _userQueryHandler.Handle(CurrentClientId, CurrentUserId);
        if (user is null) return NotFound();

        return Ok(user);
    }
}
