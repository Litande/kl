using Plat4Me.DialSipBridge.Application.Models.Messages;
using Plat4Me.DialSipBridge.Application.Services;

namespace Plat4Me.DialSipBridge.Application.Handlers;

public class AgentReplaceDataHandler : IAgentReplaceDataHandler
{
    private readonly ICallService _callService;

    public AgentReplaceDataHandler(ICallService callService)
    {
        _callService = callService;
    }

    public async Task Process(AgentReplacedMessage message, CancellationToken ct = default)
    {
        var session = _callService.GetSession(message.SessionId);
        if (session is not null)
        {
            await session.ReplaceAgent(message.AgentId);
        }
    }
}
