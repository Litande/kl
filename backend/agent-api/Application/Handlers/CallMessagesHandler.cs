﻿using KL.Agent.API.Application.Models.Messages;
using KL.Agent.API.Application.Services;

namespace KL.Agent.API.Application.Handlers;

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
