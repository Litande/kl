using System.Collections.Concurrent;
using KL.Agent.API.Application.Common;
using KL.Agent.API.Application.Enums;
using KL.Agent.API.Application.Handlers;
using KL.Agent.API.Application.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace KL.Agent.API.SignalR;

[Authorize(Roles = "Agent")]
public class AgentHub : Hub<IBaseClient>
{
    private static readonly ConcurrentDictionary<string, HubCallerContext> _clients = new();
    private const string GroupNameFormat = "client_id_{0}_group";

    public static string GetGroupName(long clientId) => string.Format(GroupNameFormat, clientId);

    public override async Task OnConnectedAsync()
    {
        AgentConnected();
        await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(CurrentClientId));

        var agentConnectedHandler = GetRequiredService<IAgentConnectedHandler>();

        await base.OnConnectedAsync();
        await agentConnectedHandler.Handle(CurrentClientId, CurrentUserId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _clients.TryRemove(Context.UserIdentifier!, out var _);
        var handler = GetRequiredService<IAgentDisconnectedHandler>();

        await base.OnDisconnectedAsync(exception);
        await handler.Handle(CurrentClientId, CurrentUserId);
    }

    public async Task<HubResponse> ManualCall(string phone)
    {
        var handler = GetRequiredService<IManualCallHandler>();

        return await handler.Handle(CurrentClientId, CurrentUserId, phone);
    }

    public async Task<HubResponse> ChangeStatus(AgentStatusTypes status)
    {
        var handler = GetRequiredService<IAgentChangeStatusRequestHandler>();

        return await handler.Handle(CurrentClientId, CurrentUserId, status);
    }

    public async Task<HubResponse> CallAgain(object? _)
    {
        var handler = GetRequiredService<ICallAgainHandler>();

        return await handler.Handle(CurrentClientId, CurrentUserId);
    }

    private T GetRequiredService<T>() where T : notnull
    {
        return Context.GetHttpContext()!.RequestServices
            .GetRequiredService<T>();
    }

    private void AgentConnected()
    {
        if (_clients.TryRemove(Context.UserIdentifier!, out var prevContext))
        {
            prevContext.Abort();
        }
        if (!_clients.TryAdd(Context.UserIdentifier!, Context))
        {
            throw new InvalidOperationException("Can not add signalr connection");
        }
    }

    public static void DisconnectAgent(long agentId)
    {
        var agentIdStr = agentId.ToString();
        _clients.TryRemove(agentIdStr, out var context);
        context?.Abort();
    }

    protected long CurrentClientId
    {
        get
        {
            var clientId = Context.User?.Claims
                .FirstOrDefault(c => c.Type == CustomClaimTypes.ClientId);

            if (clientId is null)
                throw new KeyNotFoundException("ClientId not in claims");

            return long.Parse(clientId.Value);
        }
    }

    protected long CurrentUserId
    {
        get
        {
            var userId = Context.UserIdentifier;

            if (userId is null)
                throw new KeyNotFoundException("UserId not in claims");

            return long.Parse(userId);
        }
    }
}
