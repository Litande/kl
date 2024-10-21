using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plat4Me.DialAgentApi.Application.Enums;
using Plat4Me.DialAgentApi.Application.Extensions;
using Plat4Me.DialAgentApi.Application.Models.Requests;
using Plat4Me.DialAgentApi.Application.Services;
using Plat4Me.DialAgentApi.Persistent.Entities.Settings;
using Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialAgentApi.Application.Handlers;

public class FeedbackTimeoutHandler : IFeedbackTimeoutHandler
{
    private readonly IAgentTimeoutService _agentTimeoutService;
    private readonly ISettingsRepository _settingsRepository;
    private readonly ICDRRepository _cdrRepository;
    private readonly IServiceScopeFactory _scopeFactory;

    public FeedbackTimeoutHandler(
        IAgentTimeoutService agentTimeoutService,
        ISettingsRepository settingsRepository,
        IServiceScopeFactory scopeFactory,
        ICDRRepository cdrRepository)
    {
        _agentTimeoutService = agentTimeoutService;
        _settingsRepository = settingsRepository;
        _scopeFactory = scopeFactory;
        _cdrRepository = cdrRepository;
    }

    public async Task Handle(
        long clientId,
        long agentId,
        string sessionId,
        DateTimeOffset callFinishedAt,
        CancellationToken ct = default)
    {
        var settings = await GetFeedbackSettings(clientId, ct);

        async Task OnTimeout()
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var feedbackHandler = scope.ServiceProvider.GetRequiredService<IAgentFilledCallInfoHandler>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<FeedbackTimeoutHandler>>();
            try
            {
                await feedbackHandler.Handle(
                    clientId,
                    agentId,
                    new AgentFilledCallRequest(sessionId, settings.DefaultStatus, null),
                    isGenerated: true,
                    ct);

                logger.LogInformation("FeedbackTimeout executed for client Id {clientId}; agent Id {agentId}",
                    clientId, agentId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "FeedbackTimeout failed");
            }
        }

        var feedbackOn = callFinishedAt + TimeSpan.FromSeconds(settings.PageTimeout);
        if (feedbackOn <= DateTimeOffset.UtcNow)
        {
            await OnTimeout();
        }
        else
        {
            var delay = feedbackOn - DateTimeOffset.UtcNow;
            _agentTimeoutService.SetTimeout(
                AgentTimeoutTypes.FeedbackTimeout,
                sessionId,
                (long)(delay.TotalMilliseconds), // sec to msec
                async () => await OnTimeout());
        }
    }


    public async Task Handle()
    {
        var records = await _cdrRepository.GetRecordsForFeedBack();
        foreach (var record in records)
        {
            await Handle(record.ClientId, record.AgentId, record.SessionId, record.CallFinishedAt);
        }
    }

    private async Task<FeedbackSettings> GetFeedbackSettings(long clientId, CancellationToken ct = default)
    {
        var settingsJson = await _settingsRepository.GetValue(SettingTypes.Feedback, clientId, ct);
        if (string.IsNullOrWhiteSpace(settingsJson))
            throw new ArgumentNullException(nameof(settingsJson),
                $"The {nameof(SettingTypes.Feedback)} JSON cannot be null");

        var settings = settingsJson.FeedbackSettingsDeserialize();
        if (settings is null)
            throw new ArgumentNullException(nameof(settings),
                $"The {nameof(FeedbackSettings)} settings cannot be null");

        return settings;
    }
}
