using Plat4Me.DialSipBridge.Application.Enums;

namespace Plat4Me.DialSipBridge.Application.Models.Messages;

public record CalleeAnsweredMessage(
    long ClientId,
    string BridgeId,
    string SessionId,
    CallType CallType,
    DateTimeOffset? AgentAnswerAt,
    DateTimeOffset? LeadAnswerAt,
    DateTimeOffset CallOriginatedAt,
    long AgentId,
    long? QueueId,
    long? LeadId,
    string LeadPhone,
    string ManagerRtcUrl,
    string AgentRtcUrl,
    long SipProviderId)
{
    public string Initiator => nameof(DialSipBridge);
}
