using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;
using Plat4Me.DialAgentApi.Application.Models.Messages;
using Plat4Me.DialAgentApi.Application.Models.Responses;
using System.Text.Json;
using Plat4Me.DialAgentApi.Application.Configurations;
using Plat4Me.DialAgentApi.Application.Enums;
using Plat4Me.DialAgentApi.Application.Common;
using Plat4Me.DialAgentApi.Application.Services;
using Plat4Me.DialAgentApi.Persistent.Entities.Settings;
using Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialAgentApi.Application.Handlers;

public class CallAgainHandler : ICallAgainHandler
{
    private readonly INatsPublisher _natsPublisher;
    private readonly ILogger<CallAgainHandler> _logger;
    private readonly ISettingsRepository _settingsRepository;
    private readonly NatsPubSubOptions _natsPubSubOptions;
    private readonly IAgentStateService _agentStateService;
    private readonly IAgentTimeoutService _agentTimeoutService;

    public CallAgainHandler(
        INatsPublisher natsPublisher,
        ILogger<CallAgainHandler> logger,
        ISettingsRepository settingsRepository,
        IAgentStateService agentStateService,
        IOptions<NatsPubSubOptions> natsPubSubOptions,
        IAgentTimeoutService agentTimeoutService)
    {
        _natsPublisher = natsPublisher;
        _logger = logger;
        _settingsRepository = settingsRepository;
        _natsPubSubOptions = natsPubSubOptions.Value;
        _agentStateService = agentStateService;
        _agentTimeoutService = agentTimeoutService;
    }

    public async Task<HubResponse> Handle(long clientId, long agentId)
    {
        var agentState = await _agentStateService.GetAgentCurrentState(agentId, clientId);
        if (agentState.CallInfo is null)
            return HubResponse.CreateError("Can't find callinfo", HubErrorCode.BadRequest);
        var callInfo = agentState.CallInfo;

        var settings = await GetFeedbackSettings(clientId);

        if (callInfo.CallType != CallType.Predictive
            || callInfo.CallOriginatedAt is null
            || string.IsNullOrEmpty(callInfo.SessionId)
        )
            return HubResponse.CreateError("Can't find callinfo", HubErrorCode.BadRequest);

        if (callInfo.CallFinishedAt is null)
            return HubResponse.CreateError("Call in progress", HubErrorCode.BadRequest);

        if (settings.RedialsLimit != 0 && callInfo.CallAgainCount >= settings.RedialsLimit)
            return HubResponse.CreateError("Limit reached", HubErrorCode.BadRequest);

        _agentTimeoutService.TryCancelTimeout(AgentTimeoutTypes.FeedbackTimeout, callInfo.SessionId);

        var message = new CallAgainMessage(
            clientId,
            agentId,
            callInfo.SessionId);

        await _natsPublisher.PublishAsync(_natsPubSubOptions.CallAgain, message);

        _logger.LogInformation("Publish agent update message: {message}", message);
        return HubResponse.CreateSuccess();
    }

    private async Task<FeedbackSettings> GetFeedbackSettings(long clientId, CancellationToken ct = default)
    {
        var settingsJson = await _settingsRepository.GetValue(SettingTypes.Feedback, clientId, ct);
        if (string.IsNullOrWhiteSpace(settingsJson))
            throw new ArgumentNullException(nameof(settingsJson), $"The {nameof(SettingTypes.Feedback)} JSON cannot be null");

        var settings = JsonSerializer.Deserialize<FeedbackSettings>(settingsJson, JsonSettingsExtensions.Default);
        if (settings is null)
            throw new ArgumentNullException(nameof(settings), $"The {nameof(FeedbackSettings)} settings cannot be null");

        return settings;
    }
}
