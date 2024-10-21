using Plat4Me.DialLeadCaller.Application.Handlers.Contracts;
using Plat4Me.DialLeadCaller.Application.Models.Messages;
using Plat4Me.DialLeadCaller.Application.Services.Contracts;

namespace Plat4Me.DialLeadCaller.Application.Handlers;

public class LeadFeedbackFilledHandler : ILeadFeedbackFilledHandler
{
    private readonly ICallInfoService _callInfoService;
    private readonly ICDRService _cdrService;

    public LeadFeedbackFilledHandler(
        ICallInfoService callInfoService,
        ICDRService cdrService
    )
    {
        _callInfoService = callInfoService;
        _cdrService = cdrService;
    }

    public async Task Process(LeadFeedbackFilledMessage message, CancellationToken ct = default)
    {
        await _cdrService.Update(message.SessionId, message.LeadStatus, ct);
        await _callInfoService.ClearCallInfo(message.SessionId, ct);
    }
}
