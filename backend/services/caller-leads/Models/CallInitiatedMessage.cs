using KL.Caller.Leads.Enums;

namespace KL.Caller.Leads.Models;

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
    public string Initiator => nameof(KL.Caller);
}
