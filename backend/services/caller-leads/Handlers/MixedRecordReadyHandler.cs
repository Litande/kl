using Plat4Me.DialLeadCaller.Application.Handlers.Contracts;
using Plat4Me.DialLeadCaller.Application.Models.Messages;
using Plat4Me.DialLeadCaller.Application.Repositories;
using Plat4Me.DialLeadCaller.Application.Services.Contracts;

namespace Plat4Me.DialLeadCaller.Application.Handlers;

public class MixedRecordReadyHandler : IMixedRecordReadyHandler
{
    private readonly ICDRService _cdrService;

    public MixedRecordReadyHandler(ICDRService cdrService)
    {
        _cdrService = cdrService;
    }

    public async Task Process(MixedRecordReadyMessage message, CancellationToken ct = default)
    {
        await _cdrService.Update(message, ct);
    }
}