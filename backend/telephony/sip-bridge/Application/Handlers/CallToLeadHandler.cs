using KL.SIP.Bridge.Application.Models.Messages;
using KL.SIP.Bridge.Application.Services;
using KL.SIP.Bridge.Application.Session;

namespace KL.SIP.Bridge.Application.Handlers;

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
