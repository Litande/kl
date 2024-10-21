using Microsoft.Extensions.Logging;
using Plat4Me.DialLeadCaller.Application.Models;
using Plat4Me.DialLeadCaller.Application.Enums;
using Plat4Me.DialLeadCaller.Application.Extensions;
using Plat4Me.DialLeadCaller.Application.Models.Messages;
using Plat4Me.DialLeadCaller.Application.Repositories;
using Plat4Me.DialLeadCaller.Application.Services.Contracts;
using Medallion.Threading;

namespace Plat4Me.DialLeadCaller.Application.Services;

public class CallInfoService : ICallInfoService
{
    private readonly ILogger<CallInfoService> _logger;
    private readonly ICallInfoCacheRepository _callInfoCacheRepository;
    private readonly IDistributedLockProvider _lockProvider;
    private readonly ILeadCacheRepository _leadCacheRepository;

    public CallInfoService(
        ILogger<CallInfoService> logger,
        ICallInfoCacheRepository callInfoCacheRepository,
        ILeadCacheRepository leadCacheRepository,
        IDistributedLockProvider lockProvider
    )
    {
        _logger = logger;
        _callInfoCacheRepository = callInfoCacheRepository;
        _leadCacheRepository = leadCacheRepository;
        _lockProvider = lockProvider;
    }

    public async Task UpdateCallInfo(CallFinishedMessage message, CancellationToken ct = default)
    {
        await using (await _lockProvider.AcquireLockAsync(_callInfoCacheRepository.LockPrefix + message.SessionId))
        {
            var callCache = await _callInfoCacheRepository.GetCallInfo(message.SessionId);
            if (callCache is null)
            {
                _logger.LogError("UpdateCallInfo/CalleeAnsweredMessage: Missing call info cache {sessionId} ", message.SessionId);
                return;
            }
            callCache.LeadId = message.LeadId;
            callCache.LeadPhone = message.LeadPhone;
            callCache.QueueId = message.QueueId;
            callCache.CallType = message.CallType;
            callCache.BridgeId = message.BridgeId;
            callCache.CallOriginatedAt = message.CallOriginatedAt.ToUnixTimeSeconds();
            callCache.AgentAnsweredAt = message.AgentAnswerAt?.ToUnixTimeSeconds();
            callCache.LeadAnsweredAt = message.LeadAnswerAt?.ToUnixTimeSeconds();
            callCache.CallFinishedAt = message.CallHangupAt.ToUnixTimeSeconds();
            callCache.ManagerRtcUrl = null;
            callCache.AgentRtcUrl = null;
            callCache.CallFinishReason = message.ReasonCode;
            callCache.CallStatus = CallStatusTypesExtension.Success.Contains(message.ReasonCode) ? CallStatusType.Finished : CallStatusType.Failed;
            await _callInfoCacheRepository.UpdateCallInfo(callCache);
        }
    }

    public async Task UpdateCallInfo(CalleeAnsweredMessage message, CancellationToken ct = default)
    {
        await using (await _lockProvider.AcquireLockAsync(_callInfoCacheRepository.LockPrefix + message.SessionId))
        {
            var callCache = await _callInfoCacheRepository.GetCallInfo(message.SessionId);
            if (callCache is null)
            {
                _logger.LogError("UpdateCallInfo/CalleeAnsweredMessage: Missing call info cache {sessionId} ", message.SessionId);
                return;
            }
            callCache.LeadId = message.LeadId;
            callCache.LeadPhone = message.LeadPhone;
            callCache.QueueId = message.QueueId;
            callCache.CallType = message.CallType;
            callCache.BridgeId = message.BridgeId;
            callCache.CallOriginatedAt = message.CallOriginatedAt.ToUnixTimeSeconds();
            callCache.AgentAnsweredAt = message.AgentAnswerAt?.ToUnixTimeSeconds();
            callCache.LeadAnsweredAt = message.LeadAnswerAt?.ToUnixTimeSeconds();
            callCache.CallFinishedAt = null;
            callCache.ManagerRtcUrl = message.ManagerRtcUrl;
            callCache.AgentRtcUrl = message.AgentRtcUrl;
            if (message.CallType == CallType.Manual)
            {
                if (message.AgentAnswerAt is not null)
                    callCache.CallStatus = CallStatusType.Dialing;
                if (message.LeadAnswerAt is not null)
                    callCache.CallStatus = CallStatusType.InProgress;
            }
            else if (message.CallType == CallType.Predictive)
            {
                if (message.LeadAnswerAt is not null)
                    callCache.CallStatus = CallStatusType.Dialing;
                if (message.AgentAnswerAt is not null)
                    callCache.CallStatus = CallStatusType.InProgress;
            }
            await _callInfoCacheRepository.UpdateCallInfo(callCache);
        }
    }

    public async Task AddCallInfo(CallInitiatedMessage message, CancellationToken ct = default)
    {
        var callCache = new CallInfoCache(message.SessionId, message.AgentId, message.ClientId)
        {
            CallType = message.CallType,
            BridgeId = message.BridgeId,
            QueueId = message.QueueId,
            LeadId = message.LeadId,
            LeadPhone = message.LeadPhone,
            CallStatus = CallStatusType.Initiated
        };

        if (message.LeadId.HasValue)
        {
            var leadCache = await _leadCacheRepository.GetLead(message.LeadId.Value);
            callCache.LeadScore = leadCache?.Score;
        }

        await _callInfoCacheRepository.UpdateCallInfo(callCache);
    }

    public async Task UpdateCallInfo(DroppedAgentMessage message, CancellationToken ct = default)
    {
        await using (await _lockProvider.AcquireLockAsync(_callInfoCacheRepository.LockPrefix + message.SessionId))
        {
            var callCache = await _callInfoCacheRepository.GetCallInfo(message.SessionId);
            if (callCache is null)
            {
                _logger.LogError("UpdateCallInfo/CalleeAnsweredMessage: Missing call info cache {sessionId} ", message.SessionId);
                return;
            }
            callCache.AgentDropped = true;
            await _callInfoCacheRepository.UpdateCallInfo(callCache);
        }
    }

    public async Task ClearCallInfo(string sessionId, CancellationToken ct = default)
    {
        await using (await _lockProvider.AcquireLockAsync(_callInfoCacheRepository.LockPrefix + sessionId))
        {
            await _callInfoCacheRepository.RemoveCallInfo(sessionId);
        }
    }
}
