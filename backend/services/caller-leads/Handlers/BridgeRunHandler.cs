using KL.Caller.Leads.Handlers.Contracts;
using KL.Caller.Leads.Models.Messages;
using KL.Caller.Leads.Services.Contracts;

namespace KL.Caller.Leads.Handlers;

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
