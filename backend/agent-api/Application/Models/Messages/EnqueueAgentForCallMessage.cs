using Plat4Me.DialAgentApi.Application.Enums;

namespace Plat4Me.DialAgentApi.Application.Models.Messages;

public record EnqueueAgentForCallMessage(
    long ClientId,
    long AgentId,
    CallType callType)
{
    public string Initiator => nameof(DialAgentApi);
}
