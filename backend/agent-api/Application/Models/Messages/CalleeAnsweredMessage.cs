using KL.Agent.API.Application.Enums;

namespace KL.Agent.API.Application.Models.Messages;

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
