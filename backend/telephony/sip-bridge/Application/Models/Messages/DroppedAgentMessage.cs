using KL.SIP.Bridge.Application.Enums;

namespace KL.SIP.Bridge.Application.Models.Messages;

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
