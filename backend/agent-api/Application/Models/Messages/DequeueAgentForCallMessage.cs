namespace Plat4Me.DialAgentApi.Application.Models.Messages;

public record DequeueAgentForCallMessage(
    long ClientId,
    long AgentId)
{
    public string Initiator => nameof(DialAgentApi);
}
