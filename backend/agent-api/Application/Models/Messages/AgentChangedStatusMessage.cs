using Plat4Me.DialAgentApi.Application.Enums;

namespace Plat4Me.DialAgentApi.Application.Models.Messages;

public record AgentChangedStatusMessage(
    long ClientId,
    long AgentId,
    AgentStatusTypes AgentStatus,
    DateTimeOffset SendDateTime,
    CallInfo? CallInfo
    )
{
    public string Initiator => nameof(DialAgentApi);
}
