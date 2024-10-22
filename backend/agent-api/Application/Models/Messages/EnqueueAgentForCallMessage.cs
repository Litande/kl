using KL.Agent.API.Application.Enums;

namespace KL.Agent.API.Application.Models.Messages;

public record EnqueueAgentForCallMessage(
    long ClientId,
    long AgentId,
    CallType callType)
{
    public string Initiator => nameof(DialAgentApi);
}
