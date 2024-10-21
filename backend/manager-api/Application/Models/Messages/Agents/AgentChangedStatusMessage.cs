using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Application.Models.Messages.Agents;

public record AgentChangedStatusMessage(
    long ClientId,
    long AgentId,
    AgentStatusTypes AgentStatus,
    DateTimeOffset SendDateTime,
    CallInfo? CallInfo,
    string Initiator
    );
