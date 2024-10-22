using KL.SIP.Bridge.Application.Enums;
using KL.SIP.Bridge.Application.Models.Messages;
using KL.SIP.Bridge.Application.Services;
using KL.SIP.Bridge.Application.Session;

namespace KL.SIP.Bridge.Application.Handlers;

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
