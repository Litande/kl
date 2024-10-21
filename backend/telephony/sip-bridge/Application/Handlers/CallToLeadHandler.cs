using Plat4Me.DialSipBridge.Application.Models.Messages;
using Plat4Me.DialSipBridge.Application.Services;
using Plat4Me.DialSipBridge.Application.Session;

namespace Plat4Me.DialSipBridge.Application.Handlers;

public class CallToLeadHandler: ICallToLeadHandler
{
    private readonly ICallService _callService;

    public CallToLeadHandler(ICallService callService)
    {
        _callService = callService;
    }

    public async Task Process(CallToLeadMessage message, CancellationToken ct = default)
    {
        await _callService.CreateSession(message.SessionId,
              new InitCallData(
                message.ClientId,
                message.CallType,
                message.QueueId,
                message.LeadId,
                message.Phone,
                message.AgentId,
                message.IsFixedAssigned,
                message.RingingTimeout,
                message.MaxCallDuration,
                message.IceServers,
                message.IsTest,
                message.SipProvider));
    }
}
