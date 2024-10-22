using KL.Agent.API.Application.Enums;

namespace KL.Agent.API.Application.Models.Messages;

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
    long SipProviderId);
