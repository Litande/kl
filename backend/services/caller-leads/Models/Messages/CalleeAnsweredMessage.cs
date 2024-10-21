using Plat4Me.DialLeadCaller.Application.Enums;

namespace Plat4Me.DialLeadCaller.Application.Models.Messages;

public record CalleeAnsweredMessage(
    long ClientId,
    string BridgeId,
    string SessionId,
    CallType CallType,
    DateTimeOffset? AgentAnswerAt,
    DateTimeOffset? LeadAnswerAt,
    DateTimeOffset CallOriginatedAt,
    long AgentId,
    long? LeadId,
    long? QueueId,
    string LeadPhone,
    string ManagerRtcUrl,
    string AgentRtcUrl,
    string Initiator,
    long SipProviderId);
