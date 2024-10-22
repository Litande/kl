using KL.Agent.API.Application.Enums;

namespace KL.Agent.API.Application.Models.Messages;

public record AgentChangedStatusMessage(
    long ClientId,
    long AgentId,
    AgentStatusTypes AgentStatus,
    DateTimeOffset SendDateTime,
    CallInfo? CallInfo
    )
{
    public string Initiator => nameof(KL.Agent.API);
}
