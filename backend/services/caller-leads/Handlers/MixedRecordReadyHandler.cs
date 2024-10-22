using KL.Caller.Leads.Handlers.Contracts;
using KL.Caller.Leads.Models.Messages;
using KL.Caller.Leads.Services.Contracts;

namespace KL.Caller.Leads.Handlers;

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