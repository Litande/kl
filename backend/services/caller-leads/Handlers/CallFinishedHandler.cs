using KL.Caller.Leads.Enums;
using KL.Caller.Leads.Extensions;
using KL.Caller.Leads.Handlers.Contracts;
using KL.Caller.Leads.Models.Entities.Settings;
using KL.Caller.Leads.Models.Messages;
using KL.Caller.Leads.Models.Requests;
using KL.Caller.Leads.Repositories;
using KL.Caller.Leads.Services.Contracts;

namespace KL.Caller.Leads.Handlers;

public class CallFinishedHandler : ICallFinishedHandler
{
    private readonly ILeadRepository _leadRepository;
    private readonly ILogger<CallFinishedHandler> _logger;
    private readonly ICDRService _cdrService;
    private readonly IQueueLeadsCacheRepository _queueLeadsCacheRepository;
    private readonly ILeadStatisticProcessing _leadStatisticProcessing;
    private readonly ISettingsRepository _settingsRepository;
    private readonly IAgentFilledCallInfoHandler _agentFilledCallInfoHandler;
    private readonly ICallInfoService _callInfoService;

    public CallFinishedHandler(
        ILeadRepository leadRepository,
        ILogger<CallFinishedHandler> logger,
        ICDRService cdrService,
        IQueueLeadsCacheRepository queueLeadsCacheRepository,
        ILeadStatisticProcessing leadStatisticProcessing,
        ISettingsRepository settingsRepository,
        IAgentFilledCallInfoHandler agentFilledCallInfoHandler,
        ICallInfoService callInfoService
        )
    {
        _leadRepository = leadRepository;
        _logger = logger;
        _cdrService = cdrService;
        _queueLeadsCacheRepository = queueLeadsCacheRepository;
        _leadStatisticProcessing = leadStatisticProcessing;
        _settingsRepository = settingsRepository;
        _agentFilledCallInfoHandler = agentFilledCallInfoHandler;
        _callInfoService = callInfoService;
    }

    public async Task Process(CallFinishedMessage message, CancellationToken ct = default)
    {
        await _cdrService.Update(message, ct);
        await _callInfoService.UpdateCallInfo(message, ct);

        // handle case when call failed during the exists sip connection
        if (CallStatusTypesExtension.Success.Contains(message.ReasonCode))
        {
            await UpdateLead(message, ct);
            await TryFillFeedback(message, ct);
        }
        if (message.CallType == CallType.Manual)
            await _callInfoService.ClearCallInfo(message.SessionId, ct);

        // Refresh lead statistics
        await _leadStatisticProcessing.Process(message.ClientId, ct);
    }

    private async Task UpdateLead(CallFinishedMessage message, CancellationToken ct = default)
    {
        if (!message.LeadId.HasValue) return;

        var status = await GetLeadStatus(message, ct);

        var lead = await _leadRepository.UpdateStatusAndGet(
            message.ClientId,
            message.LeadId.Value,
            LeadSystemStatusTypes.WaitingFeedback,
            status,
            ct);

        if (lead is null)
        {
            _logger.LogInformation("Cannot apply system status {LeadSystemStatus} for not found lead Id {LeadId}",
                LeadSystemStatusTypes.WaitingFeedback, message.LeadId);

            return;
        }

        await _queueLeadsCacheRepository.UpdateStatus(
            message.ClientId,
            message.QueueId!.Value,
            message.LeadId!.Value,
            systemStatus: LeadSystemStatusTypes.PostProcessing,
            status,
            ct: ct);
    }

    private async Task<LeadStatusTypes?> GetLeadStatus(CallFinishedMessage message, CancellationToken ct = default)
    {
        var isCallFinishedByManger = message.ReasonCode == CallFinishReasons.CallFinishedByManager;

        if (!isCallFinishedByManger) return null;

        var settingJson = await _settingsRepository
            .GetValue(SettingTypes.ManagerFinishCall, message.ClientId, ct);
        var setting = JsonHelper.Deserialize<ManagerFinishCallSettings>(settingJson, message.ClientId, _logger);
        return setting?.StatusAfterCall;
    }

    private async Task<bool> TryFillFeedback(CallFinishedMessage message, CancellationToken ct = default)
    {
        if (message.CallType is CallType.Predictive && message.AgentAnswerAt is not null)
        {
            if (message.ReasonCode is CallFinishReasons.CallFinishedByAgent && message.ReasonDetails == "voicemail")
            {
                await VoiceMailFeedback(message.ClientId, message.AgentId, message.SessionId, ct);
                return true;
            }
            if (message.ReasonCode is CallFinishReasons.CallFinishedByAgent && message.ReasonDetails == "na")
            {
                await NAStatusFeedback(message.ClientId, message.AgentId, message.SessionId, message.AgentComment, ct);
                return true;
            }
        }
        return false;
    }

    private async Task VoiceMailFeedback(
        long clientId,
        long agentId,
        string sessionId,
        CancellationToken ct = default
    )
    {
        var settingsJson = await _settingsRepository.GetValue(SettingTypes.VoiceMail, clientId, ct);

        if (settingsJson is null)
            throw new KeyNotFoundException($"Cannot find settings {SettingTypes.VoiceMail} with client id: {clientId}");

        var settings = JsonHelper.Deserialize<VoiceMailSettings>(settingsJson, clientId, _logger);

        var filledCallRequest = new AgentFilledCallRequest(
            sessionId,
            settings!.DefaultVoicemailStatus,
            null
        );

        await _agentFilledCallInfoHandler.Handle(clientId, agentId, filledCallRequest, ct: ct);
    }

    private async Task NAStatusFeedback(
        long clientId,
        long agentId,
        string sessionId,
        string? agentComment,
        CancellationToken ct = default
    )
    {
        var settingsJson = await _settingsRepository.GetValue(SettingTypes.CallNa, clientId, ct);

        var setting = JsonHelper.Deserialize<CallNaSettings>(settingsJson, clientId, _logger);

        var filledCallRequest = new AgentFilledCallRequest(
            sessionId,
            setting!.DefaultNaStatus,
            null,
            agentComment
        );

        await _agentFilledCallInfoHandler.Handle(clientId, agentId, filledCallRequest, ct: ct);
    }

}
