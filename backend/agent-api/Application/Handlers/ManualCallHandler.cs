using KL.Agent.API.Application.Configurations;
using KL.Agent.API.Application.Enums;
using KL.Agent.API.Application.Models.Messages;
using KL.Agent.API.Application.Models.Responses;
using KL.Agent.API.Application.Services;
using KL.Nats;
using Microsoft.Extensions.Options;

namespace KL.Agent.API.Application.Handlers;

public class ManualCallHandler : IManualCallHandler
{
    private readonly IAgentStateService _agentStateService;
    private readonly NatsPubSubOptions _natsPubSubOptions;
    private readonly INatsPublisher _natsPublisher;
    private readonly ILogger<ManualCallHandler> _logger;

    public ManualCallHandler(
        IAgentStateService agentStateService,
        IOptions<NatsPubSubOptions> natsPubSubOptions,
        INatsPublisher natsPublisher,
        ILogger<ManualCallHandler> logger)
    {
        _agentStateService = agentStateService;
        _natsPublisher = natsPublisher;
        _logger = logger;
        _natsPubSubOptions = natsPubSubOptions.Value;
    }

    public async Task<HubResponse> Handle(long clientId, long agentId, string phone, CancellationToken ct = default)
    {
        if (!await _agentStateService.CanStartManualCall(agentId, clientId))
        {
            return HubResponse.CreateError($"Agent isn't in {nameof(AgentStatusTypes.InBreak)} or {nameof(AgentStatusTypes.Offline)} status", HubErrorCode.Conflict);
        }

        var manualCall = new ManualCallMessage(clientId, agentId, phone);
        await _natsPublisher.PublishAsync(_natsPubSubOptions.ManualCall, manualCall);
        _logger.LogInformation("Publish manual call message: {message}", manualCall);
        return HubResponse.CreateSuccess();
    }
}
