using KL.Caller.Leads.Handlers.Contracts;
using KL.Caller.Leads.Models.Messages;
using KL.Caller.Leads.Services.Contracts;

namespace KL.Caller.Leads.Handlers;

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
