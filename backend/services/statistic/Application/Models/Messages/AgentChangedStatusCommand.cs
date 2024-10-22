using KL.Statistics.Application.Common.Enums;

namespace KL.Statistics.Application.Models.Messages;

public record AgentsChangedStatusMessage(
    long ClientId,
    IEnumerable<AgentChangedStatusCommand> Commands,
    string Initiator);

public record AgentChangedStatusCommand(
    long AgentId,
    AgentStatusTypes AgentStatus,
    AgentCallInfo? CallInfo = null);

public record AgentCallInfo(
    long? LeadId,
    string LeadPhone,
    string? BridgeId,
    string? SessionId,
    CallType? CallType,
    DateTimeOffset CallOriginatedAt,
    DateTimeOffset? AgentAnswerAt,
    DateTimeOffset? LeadAnswerAt,
    DateTimeOffset? CallFinishedAt,
    string? ManagerRtcUrl,
    string? AgentRtcUrl);
