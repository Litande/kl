using Plat4Me.DialSipBridge.Application.Enums;
using Plat4Me.DialSipBridge.Application.Models.Messages;
using Plat4Me.DialSipBridge.Application.Services;
using Plat4Me.DialSipBridge.Application.Session;

namespace Plat4Me.DialSipBridge.Application.Handlers;

public class HangupCallHandler : IHangupCallHandler
{
    private readonly ICallService _callService;

    public HangupCallHandler(ICallService callService)
    {
        _callService = callService;
    }

    public async Task Process(HangupCallMessage message, CancellationToken ct = default)
    {
        var session = _callService.GetSession(message.SessionId);
        if (session is not null)
        {
            var cmd = new CloseCommand(message.ReasonCode ?? CallFinishReasons.CallFinishedByAgent)
            {
                ReasonDetails = message.ReasonDetails
            };
            await session.Close(cmd);
        }
    }
}
