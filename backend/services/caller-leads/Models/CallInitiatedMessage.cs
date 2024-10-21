using Plat4Me.DialLeadCaller.Application.Enums;

namespace Plat4Me.DialLeadCaller.Application.Models;

public record CallInitiatedMessage(
    long ClientId,
    string BridgeId,
    string SessionId,
    CallType CallType,
    long AgentId,
    long? QueueId,
    long? LeadId,
    string LeadPhone
    )
{
    public string Initiator => nameof(DialLeadCaller);
}
