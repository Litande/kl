using KL.Agent.API.Application.Models.Messages;
using KL.Agent.API.Application.Services;

namespace KL.Agent.API.Application.Handlers;

public class AgentBlockedHandler : IAgentBlockedHandler
{
    private readonly IHubSender _hubSender;

    public AgentBlockedHandler(IHubSender hubSender)
    {
        _hubSender = hubSender;
    }

    public async Task Process(AgentBlockedMessage message, CancellationToken ct = default)
    {
        await _hubSender.SendAgentBlocked(message.AgentId);
        _hubSender.DisconnectAgent(message.AgentId);
    }
}