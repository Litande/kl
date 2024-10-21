using Plat4Me.DialAgentApi.Application.Models.Messages;
using Plat4Me.DialAgentApi.Application.Services;

namespace Plat4Me.DialAgentApi.Application.Handlers;

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