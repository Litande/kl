using KL.Agent.API.Application.Enums;

namespace KL.Agent.API.Application.Models.Messages;

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
    DateTimeOffset CallOriginatedAt,
    DateTimeOffset? LeadAnswerAt,
    DateTimeOffset? AgentAnswerAt,
    DateTimeOffset CallHangupAt,
    string? AgentComment = null,
    string? ManagerComment = null,
    bool AgentWasDropped = false
);