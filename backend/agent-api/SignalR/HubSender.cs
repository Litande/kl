using Microsoft.AspNetCore.SignalR;
using Plat4Me.DialAgentApi.Application.Enums;
using Plat4Me.DialAgentApi.Application.Models.SignalR;
using Plat4Me.DialAgentApi.Application.Services;

namespace Plat4Me.DialAgentApi.SignalR;

public class HubSender : IHubSender
{
    private readonly IHubContext<AgentHub, IBaseClient> _hub;

    public HubSender(IHubContext<AgentHub, IBaseClient> hub)
    {
        _hub = hub;
    }

    public async Task SendCallInfo(string agentId, CallInfo message)
    {
        await _hub.Clients.User(agentId)
            .CallInfo(message);
    }

    public async Task SendCurrentStatus(string agentId, AgentStatusTypes status)
    {
        await _hub.Clients.User(agentId)
            .CurrentStatus(status);
    }

    public async Task SendAgentBlocked(long agentId)
    {
        await _hub.Clients.User(agentId.ToString())
            .AgentBlocked();
    }

    public void DisconnectAgent(long agentId)
    {
        AgentHub.DisconnectAgent(agentId);
    }
}