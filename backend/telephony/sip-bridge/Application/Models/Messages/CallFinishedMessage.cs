using Plat4Me.DialSipBridge.Application.Enums;

namespace Plat4Me.DialSipBridge.Application.Models.Messages;

public record CallFinishedMessage(
    long ClientId,
    string BridgeId,
    string SessionId,
    CallType CallType,
    long AgentId,
    long? QueueId,
    long? LeadId,
    string LeadPhone,
    CallFinishReasons ReasonCode,
    string? ReasonDetails,
    long SipProviderId,
    int? SipErrorCode,
    DateTimeOffset CallOriginatedAt,
    DateTimeOffset? LeadAnswerAt,
    DateTimeOffset? AgentAnswerAt,
    DateTimeOffset CallHangupAt,
    string? AgentComment,
    string? ManagerComment,
    bool AgentWasDropped
    )
{
    public string Initiator => nameof(DialSipBridge);
}
