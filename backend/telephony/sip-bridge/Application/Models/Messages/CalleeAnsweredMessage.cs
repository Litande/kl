using KL.SIP.Bridge.Application.Enums;

namespace KL.SIP.Bridge.Application.Models.Messages;

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
    public string Initiator => nameof(KL.SIP.Bridge);
}
