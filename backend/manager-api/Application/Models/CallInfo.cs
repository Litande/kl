using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Application.Models;

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
