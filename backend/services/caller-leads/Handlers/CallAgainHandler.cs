using Plat4Me.DialLeadCaller.Application.Handlers.Contracts;
using Plat4Me.DialLeadCaller.Application.Models.Messages;
using Plat4Me.DialLeadCaller.Application.Repositories;
using Plat4Me.DialLeadCaller.Application.Services.Contracts;

namespace Plat4Me.DialLeadCaller.Application.Handlers;

public class CallAgainHandler : ICallAgainHandler
{
    private readonly ICallerService _callerService;
    private readonly ICDRRepository _cdrRepository;
    private readonly ICallInfoCacheRepository _callInfoCacheRepository;
    private readonly ILeadRepository _leadRepository;
    private readonly ICallInfoService _callInfoService;

    public CallAgainHandler(
        ICallerService callerService,
        ICDRRepository cdrRepository,
        ICallInfoCacheRepository callInfoCacheRepository,
        ILeadRepository leadRepository,
        ICallInfoService callInfoService
    )
    {
        _callerService = callerService;
        _cdrRepository = cdrRepository;
        _callInfoCacheRepository = callInfoCacheRepository;
        _leadRepository = leadRepository;
        _callInfoService = callInfoService;
    }

    public async Task Process(CallAgainMessage message, CancellationToken ct = default)
    {
        var cdr = await _cdrRepository.GetBySessionId(message.SessionId, ct);
        if (cdr is null)
            throw new ArgumentException($"CDR was not found for {message.SessionId}");
        
        await _callInfoService.ClearCallInfo(message.SessionId);

        if (cdr.CallType != Enums.CallType.Predictive || !cdr.LeadId.HasValue)
            throw new ArgumentException($"Can not recall on non-predictive call {message.SessionId}");

        var lead = await _leadRepository.GetLeadById(message.ClientId, cdr.LeadId.Value, ct);

        if (lead is null)
            throw new ArgumentException($"Can not recall on {message.SessionId}. Lead was not found");

        await _callInfoCacheRepository.IncreaseCallAgain(message.SessionId);
        await _callerService.TryRecall(
            message.ClientId,
            message.AgentId,
            cdr.LeadQueueId!.Value,
            cdr.LeadId!.Value,
            cdr.SipProviderId,
            cdr.LeadPhone,
            lead.AssignedAgentId.HasValue,
            lead.IsTest, ct);
    }
}
