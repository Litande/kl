using Plat4Me.DialAgentApi.Application.Enums;

namespace Plat4Me.DialAgentApi.Application.Models.Messages;

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
