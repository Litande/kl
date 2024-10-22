using KL.Caller.Leads.Handlers.Contracts;
using KL.Caller.Leads.Models.Messages;
using KL.Caller.Leads.Services.Contracts;

namespace KL.Caller.Leads.Handlers;

public class DroppedAgentHandler : IDroppedAgentHandler
{
    private readonly ICDRService _cdrService;

    public DroppedAgentHandler(
        ICDRService cdrService
    )
    {
        _cdrService = cdrService;
    }

    public async Task Process(DroppedAgentMessage message, CancellationToken ct = default)
    {
        await _cdrService.Update(message, ct);
    }

}
