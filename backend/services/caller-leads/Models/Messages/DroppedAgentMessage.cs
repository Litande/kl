using KL.Caller.Leads.Enums;

namespace KL.Caller.Leads.Models.Messages;

public record DroppedAgentMessage(
    long ClientId,
    string BridgeId,
    string SessionId,
    CallType CallType,
    long? QueueId,
    long AgentId,
    long? LeadId,
    bool IsFixedAssigned,
    string LeadPhone,
    long SipProviderId,
    DateTimeOffset DroppedAt,
    long DroppedBy,
    string? Comment
    );
