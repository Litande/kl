using KL.Caller.Leads.Handlers.Contracts;
using KL.Caller.Leads.Models.Messages;
using KL.Caller.Leads.Services.Contracts;

namespace KL.Caller.Leads.Handlers;

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
