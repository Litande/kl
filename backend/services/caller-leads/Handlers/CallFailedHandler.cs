using Medallion.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;
using Plat4Me.DialLeadCaller.Application.App;
using Plat4Me.DialLeadCaller.Application.Enums;
using Plat4Me.DialLeadCaller.Application.Handlers.Contracts;
using Plat4Me.DialLeadCaller.Application.Models.Entities.Settings;
using Plat4Me.DialLeadCaller.Application.Models.Messages;
using Plat4Me.DialLeadCaller.Application.Repositories;
using Plat4Me.DialLeadCaller.Application.Services.Contracts;
using System.Text.Json;
using Plat4Me.DialLeadCaller.Application.Extensions;

namespace Plat4Me.DialLeadCaller.Application.Handlers;

public class CallFailedHandler : ICallFailedHandler
{
    private readonly ILeadRepository _leadRepository;
    private readonly INatsPublisher _natsPublisher;
    private readonly PubSubjects _pubSubjects;
    private readonly ILogger<CallFailedHandler> _logger;
    private readonly ICDRService _cdrService;
    private readonly ISettingsRepository _settingsRepository;
    private readonly IQueueLeadsCacheRepository _queueLeadsCacheRepository;
    private readonly ILeadStatisticProcessing _leadStatisticProcessing;
    private readonly IDistributedLockProvider _lockProvider;
    private readonly ICallInfoService _callInfoService;

    public CallFailedHandler(
        ILeadRepository leadRepository,
        INatsPublisher natsPublisher,
        IOptions<PubSubjects> pubSubjects,
        ILogger<CallFailedHandler> logger,
        ICDRService cdrService,
        ISettingsRepository settingsRepository,
        IQueueLeadsCacheRepository queueLeadsCacheRepository,
        IDistributedLockProvider lockProvider,
        ILeadStatisticProcessing leadStatisticProcessing,
        ICallInfoService callInfoService
        )
    {
        _leadRepository = leadRepository;
        _natsPublisher = natsPublisher;
        _pubSubjects = pubSubjects.Value;
        _logger = logger;
        _cdrService = cdrService;
        _settingsRepository = settingsRepository;
        _queueLeadsCacheRepository = queueLeadsCacheRepository;
        _lockProvider = lockProvider;
        _leadStatisticProcessing = leadStatisticProcessing;
        _callInfoService = callInfoService;
    }

    public async Task Process(CallFinishedMessage message, CancellationToken ct = default)
    {
        await using (await _lockProvider.AcquireLockAsync(_cdrService.LockPrefix + message.SessionId))
        {
            await _cdrService.Update(message, ct);
        }

        // TODO do something when agent connected and than connection dropped.
        // add variable that indicate agent connected to determinate this event and move to another handler,
        if (message.ReasonCode == CallFinishReasons.AgentNotAnswerLeadHangUp)
        {
        }

        await UpdateLead(message, ct);

        if (message.AgentAnswerAt is null || message.CallType == CallType.Manual)
        {   
            await _callInfoService.ClearCallInfo(message.SessionId, ct);
        }
        else
        {
            await _callInfoService.UpdateCallInfo(message, ct);
        }
        

        // Refresh lead statistics
        await _leadStatisticProcessing.Process(message.ClientId, ct);
    }

    private async Task UpdateLead(CallFinishedMessage message, CancellationToken ct = default)
    {
        if (!message.LeadId.HasValue)
            return;

        var newLeadStatus = await TryMapToLeadStatus(message.ClientId, message.ReasonCode, ct);

        var lead = await _leadRepository.UpdateStatusAndGet(
            message.ClientId,
            message.LeadId.Value,
            systemStatus: LeadSystemStatusTypes.PostProcessing,
            status: newLeadStatus,
            ct);

        if (lead is null)
        {
            _logger.LogInformation("Cannot apply status {leadStatus} for not found lead Id {leadId}",
                message.ReasonCode, message.LeadId);
        }
        else
        {
            await _queueLeadsCacheRepository.UpdateStatus(
                message.ClientId,
                message.QueueId!.Value,
                message.LeadId!.Value,
                systemStatus: LeadSystemStatusTypes.PostProcessing,
                status: newLeadStatus,
                ct);
        }

        var leadFeedbackFilledMessage = new LeadFeedbackCallFailedMessage(
            message.ClientId,
            message.AgentId,
            message.QueueId,
            message.SessionId,
            message.LeadId.Value,
            newLeadStatus);

        await _natsPublisher.PublishAsync(_pubSubjects.LeadFeedbackCallFailed, leadFeedbackFilledMessage);
    }

    private async Task<LeadStatusTypes> TryMapToLeadStatus(long clientId, CallFinishReasons status, CancellationToken ct = default)
    {
        var settingsJson = await _settingsRepository.GetValue(SettingTypes.CallFinishedReason, clientId, ct);
        if (string.IsNullOrWhiteSpace(settingsJson))
            throw new ArgumentNullException(nameof(settingsJson), $"The {nameof(SettingTypes.CallFinishedReason)} JSON cannot be null");

        var settings = JsonSerializer.Deserialize<CallFinishedSettings>(settingsJson, JsonSettingsExtensions.Default);
        if (settings is null)
            throw new ArgumentNullException(nameof(settings), $"The {nameof(CallFinishedSettings)} settings cannot be null");

        switch (status)
        {
            /*
             TODO handle other reasons
            case CallFinishReasons.CallFinishedByLead:
            case CallFinishReasons.CallFinishedByAgent:
            case CallFinishReasons.AgentReconnectTimeout:
            case CallFinishReasons.LeadNotAnswer:
            case CallFinishReasons.AgentTimeout:
            */

            case CallFinishReasons.LeadInvalidPhone:
                return settings.LeadInvalidPhone;

            case CallFinishReasons.NoAvailableAgents:
            case CallFinishReasons.AgentNotAnswerLeadHangUp:
                return settings.NoAvailableAgents;

            case CallFinishReasons.SessionTimeout:
            case CallFinishReasons.LeadLineBusy:
                return settings.LeadNotAnswered;

            case CallFinishReasons.Unknown:
            case CallFinishReasons.SIPTransportError:
            case CallFinishReasons.RTCTransportTimeout:
                return settings.SystemIssues;

            default:
                return settings.Default;
        }
    }
}
