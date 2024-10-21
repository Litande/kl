using Plat4Me.DialLeadCaller.Application.Enums;

namespace Plat4Me.DialLeadCaller.Application.Models;

public record CallToLeadMessage(
    long ClientId,
    string BridgeId,
    string SessionId,
    CallType CallType,
    long? QueueId,
    long? LeadId,
    bool IsFixedAssigned,
    string Phone,
    long AgentId,
    long? RingingTimeout,
    long? MaxCallDuration,
    string[]? IceServers,
    bool IsTest,
    SipProviderInfo SipProvider)
{
    public string Initiator => nameof(DialLeadCaller);
}
