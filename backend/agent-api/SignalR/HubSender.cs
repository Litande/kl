using KL.Agent.API.Application.Enums;
using KL.Agent.API.Application.Models.SignalR;
using KL.Agent.API.Application.Services;
using Microsoft.AspNetCore.SignalR;

namespace KL.Agent.API.SignalR;

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