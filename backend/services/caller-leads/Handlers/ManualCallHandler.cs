using Plat4Me.DialLeadCaller.Application.Handlers.Contracts;
using Plat4Me.DialLeadCaller.Application.Models.Messages;
using Plat4Me.DialLeadCaller.Application.Services;
using Plat4Me.DialLeadCaller.Application.Services.Contracts;

namespace Plat4Me.DialLeadCaller.Application.Handlers;

public class ManualCallHandler : IManualCallHandler
{
    private readonly ICallerService _callerService;

    public ManualCallHandler(ICallerService callerService)
    {
        _callerService = callerService;
    }

    public async Task Process(ManualCallMessage message, CancellationToken ct = default)
    {
        await _callerService.TryToCallManual(message, ct);
    }
}
