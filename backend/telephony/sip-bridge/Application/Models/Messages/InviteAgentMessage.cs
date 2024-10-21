using Plat4Me.DialSipBridge.Application.Enums;

namespace Plat4Me.DialSipBridge.Application.Models.Messages;

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
