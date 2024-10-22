namespace KL.Agent.API.Application.Models.Messages;

public record DequeueAgentForCallMessage(
    long ClientId,
    long AgentId)
{
    public string Initiator => nameof(DialAgentApi);
}
