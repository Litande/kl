using Plat4Me.DialLeadCaller.Application.Handlers.Contracts;
using Plat4Me.DialLeadCaller.Application.Models.Messages;
using Plat4Me.DialLeadCaller.Application.Repositories;
using Plat4Me.DialLeadCaller.Application.Services.Contracts;

namespace Plat4Me.DialLeadCaller.Application.Handlers;

public class CallFinishedRecordsHandler : ICallFinishedRecordsHandler
{
    private readonly ICDRService _cdrService;

    public CallFinishedRecordsHandler(ICDRService cdrService)
    {
        _cdrService = cdrService;
    }

    public async Task Process(CallFinishedRecordsMessage message, CancellationToken ct = default)
    {
        await _cdrService.Update(message, ct);
    }
}