using System.Security.Claims;
using KL.Agent.API.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KL.Agent.API.Controllers;

[ApiController]
[Authorize(Roles = "Agent")]
public class ApiAuthorizeBase : Controller
{
    private long? _currentUserId;
    private long? _currentClientId;

    protected long CurrentUserId
    {
        get
        {
            if (_currentUserId is not null)
                return _currentUserId.Value;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                throw new KeyNotFoundException("UserId not in claims");

            _currentUserId = long.Parse(userId);
            return _currentUserId.Value;
        }
    }

    protected long CurrentClientId
    {
        get
        {
            if (_currentClientId is not null)
                return _currentClientId.Value;

            var clientId = User.FindFirstValue(CustomClaimTypes.ClientId);
            if (clientId is null)
                throw new KeyNotFoundException("ClientId not in claims");

            _currentClientId = long.Parse(clientId);
            return _currentClientId.Value;
        }
    }
}
