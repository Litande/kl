using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;
using Plat4Me.DialAgentApi.Application.Configurations;
using Plat4Me.DialAgentApi.Application.Models.Messages;
using Plat4Me.DialAgentApi.Application.Models.Responses;
using Plat4Me.DialAgentApi.Application.Enums;
using Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;
using Plat4Me.DialAgentApi.Application.Services;

namespace Plat4Me.DialAgentApi.Application.Handlers;

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
