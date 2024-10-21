using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Plat4Me.DialClientApi.Application.Common;

namespace Plat4Me.DialClientApi.Controllers;

[ApiController]
[Authorize(Roles = "Manager")]
public class ApiAuthorizeBase : Controller
{
    private long? _currentClientId;
    private long? _currentUserId;

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


            var clientId = User.Claims
                    .FirstOrDefault(c => c.Type == CustomClaimTypes.ClientId);

            if (clientId is null)
                throw new KeyNotFoundException("ClientId not in claims");

            _currentClientId = long.Parse(clientId.Value);
            return _currentClientId.Value;
        }
    }
}
