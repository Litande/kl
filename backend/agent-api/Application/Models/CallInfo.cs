using KL.Agent.API.Application.Enums;

namespace KL.Agent.API.Application.Models;

public record CallInfo(
    long ClientId,
    string SessionId,
    CallType CallType,
    long? LeadId,
    string? LeadPhone,
    long? LeadScore,
    long? QueueId,
    long? CallOriginatedAt,
    long? LeadAnsweredAt,
    long? AgentAnsweredAt,
    long? CallFinishedAt,
    CallFinishReasons? CallFinishReason,
    string? ManagerRtcUrl,
    string? AgentRtcUrl,
    long? CallAgainCount
);
