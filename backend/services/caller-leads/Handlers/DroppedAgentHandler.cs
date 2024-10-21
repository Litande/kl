using Plat4Me.DialLeadCaller.Application.Handlers.Contracts;
using Plat4Me.DialLeadCaller.Application.Models.Messages;
using Plat4Me.DialLeadCaller.Application.Services.Contracts;

namespace Plat4Me.DialLeadCaller.Application.Handlers;

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
