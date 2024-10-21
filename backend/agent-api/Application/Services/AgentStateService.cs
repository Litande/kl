using Microsoft.Extensions.Options;
using Plat4Me.DialAgentApi.Application.Enums;
using Plat4Me.DialAgentApi.Application.Handlers;
using Plat4Me.DialAgentApi.Application.Models.Messages;
using Plat4Me.DialAgentApi.Application.Models;
using Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;
using Plat4Me.DialAgentApi.Persistent.Entities.Cache;
using Plat4Me.DialAgentApi.Application.Configurations;
using Plat4Me.DialAgentApi.Application.Extensions;
using Plat4Me.DialAgentApi.Persistent.Entities;
using Medallion.Threading;
using Plat4me.Core.Nats;


namespace Plat4Me.DialAgentApi.Application.Services;

public record AgentState
(
    AgentStateCache AgentInfo,
    CallInfo? CallInfo
);

public class AgentStateService : IAgentStateService
{
    private readonly IAgentCacheRepository _agentCacheRepository;
    private readonly ICallInfoCacheRepository _callInfoCacheRepository;
    private readonly IDistributedLockProvider _lockProvider;
    private readonly ICallInfoHandler _callInfoHandler;
    private readonly IAgentCurrentStatusHandler _agentCurrentStatusHandler;
    private readonly IFeedbackTimeoutHandler _feedbackTimeoutHandler;
    private readonly ILogger<AgentStateService> _logger;
    private readonly INatsPublisher _natsPublisher;
    private readonly NatsPubSubOptions _natsPubSubOptions;
    private readonly IAgentStatusHistoryRepository _agentStatusHistoryRepository;

    public AgentStateService(
        IAgentCacheRepository agentCacheRepository,
        ICallInfoCacheRepository callInfoCacheRepository,
        IDistributedLockProvider lockProvider,
        ICallInfoHandler callInfoHandler,
        IAgentCurrentStatusHandler agentCurrentStatusHandler,
        IFeedbackTimeoutHandler feedbackTimeoutHandler,
        INatsPublisher natsPublisher,
        IOptions<NatsPubSubOptions> natsPubSubOptions,
        ILogger<AgentStateService> logger,
        IAgentStatusHistoryRepository agentStatusHistoryRepository
    )
    {
        _agentCacheRepository = agentCacheRepository;
        _callInfoCacheRepository = callInfoCacheRepository;
        _lockProvider = lockProvider;
        _callInfoHandler = callInfoHandler;
        _agentCurrentStatusHandler = agentCurrentStatusHandler;
        _logger = logger;
        _natsPublisher = natsPublisher;
        _natsPubSubOptions = natsPubSubOptions.Value;
        _agentStatusHistoryRepository = agentStatusHistoryRepository;
        _feedbackTimeoutHandler = feedbackTimeoutHandler;
    }

    public async Task AgentConnected(long agentId, long clientId, CancellationToken ct = default)
    {
        await ChangeAgentStatus(agentId, clientId, AgentInternalStatusTypes.Online, true);
    }

    public async Task AgentDisconnected(long agentId, long clientId, CancellationToken ct = default)
    {
        await ChangeAgentStatus(agentId, clientId, AgentInternalStatusTypes.Offline, true);
    }

    public async Task<AgentState> GetAgentCurrentState(long agentId, long clientId, CancellationToken ct = default)
    {
        var agentCache = await _agentCacheRepository.GetAgent(agentId)
              ?? new AgentStateCache(agentId, clientId, AgentInternalStatusTypes.Offline);
        var callInfo = agentCache.CallSession is not null ? await _callInfoCacheRepository.GetCallInfo(agentCache.CallSession) : null;
        return new AgentState(agentCache, callInfo?.ToCallInfo());
    }

    public async Task ChangeAgentStatus(long agentId, long clientId, AgentInternalStatusTypes status, bool forcePublish = false, CancellationToken ct = default)
    {
        await using (await _lockProvider.AcquireLockAsync(_agentCacheRepository.LockPrefix + agentId.ToString()))
        {
            var agentCache = await _agentCacheRepository.GetAgent(agentId)
                ?? new AgentStateCache(agentId, clientId, status);
            if (status != AgentInternalStatusTypes.Online || agentCache.AgentStatus != AgentInternalStatusTypes.WaitingForTheCall) //reconnect
                agentCache.AgentStatus = status;
            var callInfoCache = agentCache.CallSession is not null ? await _callInfoCacheRepository.GetCallInfo(agentCache.CallSession) : null;
            await ProcessAgentStatus(agentCache, callInfoCache?.ToCallInfo(), forcePublish, ct);
        }
    }

    public async Task ChangeAgentStatus(long agentId, long clientId, AgentStatusTypes status, bool forcePublish = false, CancellationToken ct = default)
    {
        await ChangeAgentStatus(
            agentId,
            clientId,
            status switch
            {
                AgentStatusTypes.WaitingForTheCall => AgentInternalStatusTypes.WaitingForTheCall,
                AgentStatusTypes.Offline => AgentInternalStatusTypes.Online,
                AgentStatusTypes.InBreak => AgentInternalStatusTypes.InBreak,
                _ => throw new ArgumentException("Agent status not supported")
            },
            forcePublish,
            ct
        );
    }

    public async Task Handle(LeadFeedbackFilledMessage message, CancellationToken ct = default)
    {
        await using (await _lockProvider.AcquireLockAsync(_agentCacheRepository.LockPrefix + message.AgentId.ToString()))
        {
            var agentCache = await _agentCacheRepository.GetAgent(message.AgentId)
                ?? new AgentStateCache(message.AgentId, message.ClientId, AgentInternalStatusTypes.Online);
            agentCache.AssignedCallCount = agentCache.AssignedCallCount > 0 ? agentCache.AssignedCallCount - 1 : 0;
            if (agentCache.CallSession == message.SessionId)
            {
                agentCache.CallSession = null;
                agentCache.AgentStatus = agentCache.AgentStatus != AgentInternalStatusTypes.Offline
                    ? AgentInternalStatusTypes.Online
                    : AgentInternalStatusTypes.Offline;//wait for ChangeStatus
                agentCache.AgentDisplayStatus = GenerateAgentDisplayStatus(agentCache, null, ct);
            }
            await _agentCacheRepository.UpdateAgent(agentCache);
        }
    }

    public async Task Handle(CallInitiatedMessage message, CancellationToken ct = default)
    {
        await using (await _lockProvider.AcquireLockAsync(_agentCacheRepository.LockPrefix + message.AgentId.ToString()))
        {
            var agentCache = await _agentCacheRepository.GetAgent(message.AgentId)
                ?? new AgentStateCache(message.AgentId, message.ClientId, AgentInternalStatusTypes.Offline);

            agentCache.AssignedCallCount += 1;
            if (agentCache.AssignedCallCount == 1)//only first one, dialing status
            {
                await ProcessAgentStatus(agentCache, null, false, ct);
            }
            else
                await _agentCacheRepository.UpdateAgent(agentCache);
        }
    }

    public async Task Handle(InviteAgentMessage message, CancellationToken ct = default)
    {
        await using (await _lockProvider.AcquireLockAsync(_agentCacheRepository.LockPrefix + message.AgentId.ToString()))
        {
            var agentCache = await _agentCacheRepository.GetAgent(message.AgentId)
                ?? new AgentStateCache(message.AgentId, message.ClientId, AgentInternalStatusTypes.Offline);
            //await _agentCurrentStatusHandler.Handle(agentCache.AgentId, agentCache.AgentDisplayStatus, ct);
            await _callInfoHandler.Handle(agentCache.AgentId, message.ToCallInfo(), ct);
        }
    }

    public async Task Handle(DroppedAgentMessage message, CancellationToken ct = default)
    {
        await using (await _lockProvider.AcquireLockAsync(_agentCacheRepository.LockPrefix + message.AgentId.ToString()))
        {
            var agentCache = await _agentCacheRepository.GetAgent(message.AgentId)
                ?? new AgentStateCache(message.AgentId, message.ClientId, AgentInternalStatusTypes.Offline);
            agentCache.CallSession = null;
            agentCache.ManagerRtcUrl = null;
            agentCache.AssignedCallCount = agentCache.AssignedCallCount > 0 ? agentCache.AssignedCallCount - 1 : 0;
            await ProcessAgentStatus(agentCache, null, false, ct);
        }
    }

    public async Task Handle(AgentReplacedMessage message, CancellationToken ct = default)
    {
        await using (await _lockProvider.AcquireLockAsync(_agentCacheRepository.LockPrefix + message.AgentId.ToString()))
        {
            var agentCache = await _agentCacheRepository.GetAgent(message.AgentId)
                ?? new AgentStateCache(message.AgentId, message.ClientId, AgentInternalStatusTypes.Offline);
            agentCache.CallSession = null;
            agentCache.ManagerRtcUrl = null;
            agentCache.AssignedCallCount = agentCache.AssignedCallCount > 0 ? agentCache.AssignedCallCount - 1 : 0;
            await ProcessAgentStatus(agentCache, null, false, ct);
        }
    }

    public async Task Handle(CalleeAnsweredMessage message, CancellationToken ct = default)
    {
        await using (await _lockProvider.AcquireLockAsync(_agentCacheRepository.LockPrefix + message.AgentId.ToString()))
        {
            var agentCache = await _agentCacheRepository.GetAgent(message.AgentId)
                ?? new AgentStateCache(message.AgentId, message.ClientId, AgentInternalStatusTypes.Offline);

            if (message.CallType is CallType.Manual)
            {
                agentCache.CallSession = message.SessionId;
                agentCache.ManagerRtcUrl = message.ManagerRtcUrl;
                var callInfoCache = await _callInfoCacheRepository.GetCallInfo(message.SessionId);
                await ProcessAgentStatus(agentCache, message.ToCallInfo(callInfoCache?.LeadScore ?? 0, callInfoCache?.CallAgainCount), false, ct);
            }
            else if (message.CallType is CallType.Predictive)
            {
                if (message.AgentAnswerAt is not null) //ignore lead answer
                {
                    agentCache.CallSession = message.SessionId;
                    agentCache.ManagerRtcUrl = message.ManagerRtcUrl;
                    var callInfoCache = await _callInfoCacheRepository.GetCallInfo(message.SessionId);
                    await ProcessAgentStatus(agentCache, message.ToCallInfo(callInfoCache?.LeadScore ?? 0, callInfoCache?.CallAgainCount), false, ct);
                }
            }
        }
    }

    public async Task Handle(CallFinishedMessage message, CancellationToken ct = default)
    {
        if (message.AgentWasDropped) return;

        await using (await _lockProvider.AcquireLockAsync(_agentCacheRepository.LockPrefix + message.AgentId.ToString()))
        {
            var agentCache = await _agentCacheRepository.GetAgent(message.AgentId)
                ?? new AgentStateCache(message.AgentId, message.ClientId, AgentInternalStatusTypes.Offline);

            agentCache.AssignedCallCount = agentCache.AssignedCallCount > 0 ? agentCache.AssignedCallCount - 1 : 0;
            var callInfoCache = agentCache.CallSession is not null ? await _callInfoCacheRepository.GetCallInfo(agentCache.CallSession) : null;

            if (agentCache.CallSession == message.SessionId)
            {
                agentCache.ManagerRtcUrl = null;
                if (message.CallType == CallType.Predictive && string.IsNullOrEmpty(message.ReasonDetails))// if non voicemail or na
                {
                    await ProcessAgentStatus(agentCache, message.ToCallInfo(callInfoCache?.LeadScore ?? 0, callInfoCache?.CallAgainCount), false, ct);
                }
                else
                {
                    agentCache.CallSession = null;
                    await ProcessAgentStatus(agentCache, null, false, ct);
                }
                return;
            }

            if (agentCache.AssignedCallCount == 0)
            {
                await ProcessAgentStatus(agentCache, callInfoCache?.ToCallInfo(), false, ct);
            }
            else
                await _agentCacheRepository.UpdateAgent(agentCache);
        }
    }

    public async Task<bool> CanStartManualCall(long agentId, long clientId, CancellationToken ct = default)
    {
        var agentCache = await _agentCacheRepository.GetAgent(agentId);
        if (agentCache is null)
        {
            _logger.LogError("CanStartManualCall: agent {agentId} has no state in cache", agentId);
            return false;
        }
        if (agentCache.CallSession is not null)
        {
            _logger.LogError("CanStartManualCall: agent {agentId} has call in progress", agentId);
            return false;
        }
        if (agentCache.AgentStatus is not (AgentInternalStatusTypes.Online or AgentInternalStatusTypes.InBreak))
        {
            _logger.LogError("CanStartManualCall: agent {agentId} has invalid status {status}", agentId, agentCache.AgentStatus);
            return false;
        }
        return true;
    }

    private async Task PublishAgentStatus(AgentStateCache agentCache, CallInfo? callInfo, CancellationToken ct = default)
    {
        var message = new AgentChangedStatusMessage(
            agentCache.ClientId,
            agentCache.AgentId,
            agentCache.AgentDisplayStatus,
            DateTimeOffset.UtcNow,
            callInfo
            );

        await _natsPublisher.PublishAsync(_natsPubSubOptions.AgentChangedStatus, message);
        _logger.LogInformation("Publish agent update message: {message}", message);
    }

    private async Task ProcessAgentStatus(AgentStateCache agentCache, CallInfo? callInfo, bool forcePublish = false, CancellationToken ct = default)
    {
        var prevStatus = agentCache.AgentDisplayStatus;
        agentCache.AgentDisplayStatus = GenerateAgentDisplayStatus(agentCache, callInfo, ct);
        await _agentCacheRepository.UpdateAgent(agentCache);

        if (prevStatus != agentCache.AgentDisplayStatus || forcePublish)
        {
            if (agentCache.AgentStatus is not AgentInternalStatusTypes.Offline)
            {
                await _agentCurrentStatusHandler.Handle(agentCache.AgentId, agentCache.AgentDisplayStatus, ct);
                if (callInfo is not null)
                    await _callInfoHandler.Handle(agentCache.AgentId, callInfo, ct);
            }

            await PublishAgentStatus(agentCache, callInfo, ct);
            await _agentStatusHistoryRepository.AddStatusHistory(
                new AgentStatusHistory
                {
                    AgentId = agentCache.AgentId,
                    OldStatus = prevStatus,
                    NewStatus = agentCache.AgentDisplayStatus,
                    Initiator = nameof(DialAgentApi),
                    CreatedAt = DateTimeOffset.UtcNow,
                }, ct);

            if (agentCache.AgentDisplayStatus is AgentStatusTypes.FillingFeedback)
            {
                if (callInfo is not null)
                {
                    await _feedbackTimeoutHandler.Handle(agentCache.ClientId, agentCache.AgentId,
                        callInfo.SessionId, DateTimeOffset.FromUnixTimeSeconds(callInfo.CallFinishedAt!.Value), ct);
                }
                else
                    _logger.LogError("Missing callInfo for fillingfeedback on agent {agentId}", agentCache.AgentId);
            }
            else if (agentCache.AgentDisplayStatus is AgentStatusTypes.WaitingForTheCall)
                await EnqueueForCalls(agentCache.AgentId, agentCache.ClientId, ct);
            else if (agentCache.AgentDisplayStatus is AgentStatusTypes.Offline or AgentStatusTypes.InBreak)
                await DequeueForCalls(agentCache.AgentId, agentCache.ClientId, ct);
        }
    }

    private async Task EnqueueForCalls(long agentId, long clientId, CancellationToken ct = default)
    {
        var message = new EnqueueAgentForCallMessage(
            clientId,
            agentId,
            CallType.Predictive);

        await _natsPublisher.PublishAsync(_natsPubSubOptions.EnqueueAgentForCall, message);
        _logger.LogInformation("Publish agent enqueueforcalls message: {message}", message);
    }

    private async Task DequeueForCalls(long agentId, long clientId, CancellationToken ct = default)
    {
        var message = new DequeueAgentForCallMessage(
            clientId,
            agentId
            );

        await _natsPublisher.PublishAsync(_natsPubSubOptions.DequeueAgentForCall, message);
        _logger.LogInformation("Publish agent dequeueforcalls message: {message}", message);
    }

    private AgentStatusTypes GenerateAgentDisplayStatus(AgentStateCache agentCache, CallInfo? callInfo, CancellationToken ct = default)
    {
        _logger.LogInformation("Generate status with State: {state} CallInfo: {callInfo}", agentCache, callInfo);
        if (agentCache.AgentStatus is AgentInternalStatusTypes.Offline)
            return AgentStatusTypes.Offline;
        if (callInfo is not null)
        {
            if (callInfo.CallType is CallType.Manual)
            {
                if (callInfo.LeadAnsweredAt is null)
                    return AgentStatusTypes.Dialing;
                if (callInfo.CallFinishedAt is null)
                    return AgentStatusTypes.InTheCall;
            }

            if (callInfo.CallType is CallType.Predictive)
            {
                if (callInfo.CallFinishedAt is not null)
                    return AgentStatusTypes.FillingFeedback;
                if (callInfo.AgentAnsweredAt is null)
                    return AgentStatusTypes.Dialing;
                else
                    return AgentStatusTypes.InTheCall;
            }
        }

        if (agentCache.AssignedCallCount > 0)
            return AgentStatusTypes.Dialing;

        return agentCache.AgentStatus switch
        {
            AgentInternalStatusTypes.Online => AgentStatusTypes.Offline,
            AgentInternalStatusTypes.WaitingForTheCall => AgentStatusTypes.WaitingForTheCall,
            AgentInternalStatusTypes.InBreak => AgentStatusTypes.InBreak,
            _ => AgentStatusTypes.Offline
        };
    }

}
