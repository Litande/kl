using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Models;

public record CallInfo(
    long ClientId,
    string SessionId,
    CallType CallType,
    long? LeadId,
    string? LeadPhone,
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
