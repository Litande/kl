using Microsoft.AspNetCore.Mvc;
using Plat4Me.DialRuleEngine.Application.Extensions;

namespace Plat4Me.DialRuleEngine.Controllers;

public class BaseController : Controller
{
    protected IHttpContextAccessor ContextAccessor => HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>();

    private long? _currentClientId;

    protected long CurrentClientId
    {
        get
        {
            if (_currentClientId is not null)
                return _currentClientId.Value;

            var clientId = ContextAccessor.HttpContext.GetClientId();

            if (clientId is null)
                throw new ArgumentNullException(nameof(clientId), "ClientId not in headers");

            _currentClientId = clientId.Value;
            return _currentClientId.Value;
        }
    }
}
