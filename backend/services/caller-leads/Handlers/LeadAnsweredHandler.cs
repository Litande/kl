using KL.Caller.Leads.Handlers.Contracts;
using KL.Caller.Leads.Models.Messages;
using KL.Caller.Leads.Services.Contracts;

namespace KL.Caller.Leads.Handlers;

public class LeadAnsweredHandler : ILeadAnsweredHandler
{
    private readonly ICDRService _cdrService;
    private readonly ICallInfoService _callInfoService;
    private readonly ILogger<LeadAnsweredHandler> _logger;

    public LeadAnsweredHandler(
        ICDRService cdrService,
        ICallInfoService callInfoService,
        ILogger<LeadAnsweredHandler> logger)
    {
        _cdrService = cdrService;
        _callInfoService = callInfoService;
        _logger = logger;
    }

    public async Task Process(CalleeAnsweredMessage message, CancellationToken ct = default)
    {
        await _cdrService.Update(message, ct);
        await _callInfoService.UpdateCallInfo(message);
    }

}
