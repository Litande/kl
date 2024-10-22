using KL.SIP.Bridge.Application.Enums;

namespace KL.SIP.Bridge.Application.Models.Messages;

public record InviteAgentMessage(
    long ClientId,
    string BridgeId,
    string SessionId,
    CallType CallType,
    long AgentId,
    long? LeadId,
    string LeadPhone,
    DateTimeOffset CallOriginatedAt,
    DateTimeOffset? LeadAnsweredAt,
    string AgentRtcUrl,
    long SipProviderId)
{
    public string Initiator => nameof(DialSipBridge);
}
