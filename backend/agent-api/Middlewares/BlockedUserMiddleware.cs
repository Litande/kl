using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialAgentApi.Middlewares;

public class BlockedUserAuthorizationMiddlewareHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)

    {
        if (authorizeResult.Succeeded)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<BlockedUserAuthorizationMiddlewareHandler>>();
                logger.LogWarning("User id is null");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("User id is null");
                return;
            }
            var blockedUserCacheRepository = context.RequestServices.GetRequiredService<IBlockedUserCacheRepository>();
            var isUserBlocked = await blockedUserCacheRepository.IsUserBlocked(userId);
            if (isUserBlocked)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("User is blocked");
                return;
            }
        }

        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}