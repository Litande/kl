using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Models.Messages.Agents;

public record AgentChangedStatusMessage(
    long ClientId,
    long AgentId,
    AgentStatusTypes AgentStatus,
    DateTimeOffset SendDateTime,
    CallInfo? CallInfo,
    string Initiator
    );
