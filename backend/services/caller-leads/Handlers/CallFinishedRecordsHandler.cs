using KL.Caller.Leads.Handlers.Contracts;
using KL.Caller.Leads.Models.Messages;
using KL.Caller.Leads.Services.Contracts;

namespace KL.Caller.Leads.Handlers;

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