using Plat4Me.DialAgentApi.Application.Models.Messages;
using Plat4Me.DialAgentApi.Application.Services;

namespace Plat4Me.DialAgentApi.Application.Handlers;

public class CallMessagesHandler : ICallMessagesHandler
{
    private readonly IAgentStateService _agentStateService;

    public CallMessagesHandler(
        IAgentStateService agentStateService
    )
    {
        _agentStateService = agentStateService;
    }

    public async Task Process(InviteAgentMessage message, CancellationToken ct = default)
    {
        await _agentStateService.Handle(message, ct);
    }

    public async Task Process(CallFinishedMessage message, CancellationToken ct = default)
    {
        await _agentStateService.Handle(message, ct);
    }

    public async Task Process(CalleeAnsweredMessage message, CancellationToken ct = default)
    {
        await _agentStateService.Handle(message, ct);
    }

    public async Task Process(DroppedAgentMessage message, CancellationToken ct = default)
    {
        await _agentStateService.Handle(message, ct);
    }

    public async Task Process(AgentReplacedMessage message, CancellationToken ct = default)
    {
        await _agentStateService.Handle(message, ct);
    }

    public async Task Process(CallInitiatedMessage message, CancellationToken ct = default)
    {
        await _agentStateService.Handle(message, ct);
    }

    public async Task Process(LeadFeedbackFilledMessage message, CancellationToken ct = default)
    {
        await _agentStateService.Handle(message, ct);
    }
}
