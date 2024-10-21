using Microsoft.Extensions.Logging;
using Plat4Me.DialLeadCaller.Application.Enums;
using Plat4Me.DialLeadCaller.Application.Handlers.Contracts;
using Plat4Me.DialLeadCaller.Application.Models.Messages;
using Plat4Me.DialLeadCaller.Application.Repositories;
using Plat4Me.DialLeadCaller.Application.Services.Contracts;

namespace Plat4Me.DialLeadCaller.Application.Handlers;

public class AgentAnsweredHandler : IAgentAnsweredHandler
{
    private readonly ICDRService _cdrService;
    private readonly ILeadRepository _leadRepository;
    private readonly ILogger<AgentAnsweredHandler> _logger;
    private readonly IQueueLeadsCacheRepository _queueLeadsCacheRepository;
    private readonly ICallInfoService _callInfoService;

    public AgentAnsweredHandler(
        ICDRService cdrService,
        ILeadRepository leadRepository,
        ILogger<AgentAnsweredHandler> logger,
        IQueueLeadsCacheRepository queueLeadsCacheRepository,
        ICallInfoService callInfoService
    )
    {
        _cdrService = cdrService;
        _leadRepository = leadRepository;
        _logger = logger;
        _queueLeadsCacheRepository = queueLeadsCacheRepository;
        _callInfoService = callInfoService;
    }

    public async Task Process(CalleeAnsweredMessage message, CancellationToken ct = default)
    {
        await _cdrService.Update(message, ct);
        await UpdateLead(message, ct);
        await _callInfoService.UpdateCallInfo(message, ct);
    }


    private async Task UpdateLead(CalleeAnsweredMessage message, CancellationToken ct = default)
    {
        if (!message.LeadId.HasValue) return;

        var lead = await _leadRepository.UpdateStatusAndGet(
            message.ClientId,
            message.LeadId.Value,
            LeadSystemStatusTypes.InTheCall,
            ct: ct);

        if (lead is null)
        {
            _logger.LogWarning(
                "Cannot apply status {leadStatus} for not found lead Id {leadId}, call type {callType}",
                LeadSystemStatusTypes.InTheCall, message.LeadId, message.CallType);

            return;
        }

        await _queueLeadsCacheRepository.UpdateStatus(
            message.ClientId,
            message.QueueId!.Value,
            message.LeadId!.Value,
            LeadSystemStatusTypes.InTheCall,
            ct: ct);
    }
}
