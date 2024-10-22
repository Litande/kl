using KL.Caller.Leads.App;
using KL.Caller.Leads.Handlers.Contracts;
using KL.Caller.Leads.Models.Entities;
using KL.Caller.Leads.Models.Messages;
using KL.Caller.Leads.Repositories;
using KL.Caller.Leads.Services.Contracts;
using Microsoft.Extensions.Options;

namespace KL.Caller.Leads.Handlers;

public class AgentNotAnsweredHandler : IAgentNotAnsweredHandler
{
    private readonly INatsPublisher _natsPublisher;
    private readonly PubSubjects _pubSubjects;
    private readonly ICallerService _callerService;
    private readonly ICDRService _cdrService;
    private readonly ILogger<AgentNotAnsweredHandler> _logger;
    private readonly IDistributedLockProvider _lockProvider;
    private readonly ICallInfoCacheRepository _callInfoCacheRepository;

    public AgentNotAnsweredHandler(
        INatsPublisher natsPublisher,
        IOptions<PubSubjects> pubSubjects,
        ICallerService callerService,
        ILogger<AgentNotAnsweredHandler> logger,
        IDistributedLockProvider lockProvider,
        ICallInfoCacheRepository callInfoCacheRepository,
        ICDRService cdrService)
    {
        _natsPublisher = natsPublisher;
        _pubSubjects = pubSubjects.Value;
        _callerService = callerService;
        _logger = logger;
        _lockProvider = lockProvider;
        _callInfoCacheRepository = callInfoCacheRepository;
        _cdrService = cdrService;
    }

    public async Task Process(AgentNotAnsweredMessage message, CancellationToken ct = default)
    {
        if (message.IsFixedAssigned)
            return;

        CallDetailRecord? cdr;
        await using (await _lockProvider.AcquireLockAsync(_cdrService.LockPrefix + message.SessionId))
        {
            cdr = await _cdrService.GetBySessionId(message.SessionId, ct);
        }

        if (cdr is null)
        {
            _logger.LogWarning("Missing CDR for call with SessionID {sessionId}", message.SessionId);
            return;
        }

        if (cdr.CallHangupAt is not null)
            return;

        var freeAgentId = await _callerService.GetFreeAgentId(
            message.ClientId,
            message.QueueId,
            message.AgentId,
            message.LeadId,
            ct);

        if (!freeAgentId.HasValue)
            return;

        await using (await _lockProvider.AcquireLockAsync(_callInfoCacheRepository.LockPrefix + message.SessionId))
        {
            var callCache = await _callInfoCacheRepository.GetCallInfo(message.SessionId);
            if (callCache is not null)
                callCache.AgentId = freeAgentId.Value;
        }

        var agentReplacedMessage = new AgentReplacedMessage(
            message.ClientId,
            freeAgentId.Value,
            message.SessionId,
            message.SipProviderId);

        await _natsPublisher.PublishAsync(_pubSubjects.AgentReplaceResult, agentReplacedMessage);

        await _cdrService.Update(cdr, freeAgentId.Value, ct);
    }
}
