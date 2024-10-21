using Microsoft.Extensions.Logging;
using Plat4Me.DialLeadCaller.Application.Handlers.Contracts;
using Plat4Me.DialLeadCaller.Application.Models.Messages;
using Plat4Me.DialLeadCaller.Application.Services.Contracts;

namespace Plat4Me.DialLeadCaller.Application.Handlers;

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
