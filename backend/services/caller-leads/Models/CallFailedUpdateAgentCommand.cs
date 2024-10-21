using Plat4Me.DialLeadCaller.Application.Enums;

namespace Plat4Me.DialLeadCaller.Application.Models;

public record CallFailedUpdateAgentCommand(
    long ClientId,
    string BridgeId,
    string SessionId,
    CallType CallType,
    long AgentId,
    long? QueueId,
    long? LeadId,
    string LeadPhone,
    DateTimeOffset? CallOriginatedAt,
    DateTimeOffset? LeadAnswerAt,
    DateTimeOffset? AgentAnswerAt,
    DateTimeOffset? CallHangupAt);
