using Plat4Me.DialLeadCaller.Application.Enums;

namespace Plat4Me.DialLeadCaller.Application.Models.Messages;

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
    string? AgentComment = null,
    string? ManagerComment = null,
    bool AgentWasDropped = false)
{
    public string Initiator => nameof(DialLeadCaller);
}
