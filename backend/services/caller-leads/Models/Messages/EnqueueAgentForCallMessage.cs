using KL.Caller.Leads.Enums;

namespace KL.Caller.Leads.Models.Messages;

public record EnqueueAgentForCallMessage(
    long ClientId,
    long AgentId,
    CallType callType,
    string Initiator);
