using Plat4Me.DialLeadCaller.Application.Handlers.Contracts;
using Plat4Me.DialLeadCaller.Application.Models.Messages;
using Plat4Me.DialLeadCaller.Application.Services.Contracts;

namespace Plat4Me.DialLeadCaller.Application.Handlers;

public class BridgeRunHandler : IBridgeRunHandler
{
    private readonly IBridgeService _bridgeService;

    public BridgeRunHandler(IBridgeService bridgeService)
    {
        _bridgeService = bridgeService;
    }

    public Task Process(BridgeRunMessage message, CancellationToken ct = default)
    {
        _bridgeService.RegisterBridge(message.BridgeId, message.BridgeAddr);
        return Task.CompletedTask;
    }
}
