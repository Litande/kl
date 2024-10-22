namespace KL.Agent.API.Application.Models.Messages;

public record CallAgainMessage(
    long ClientId,
    long AgentId,
    string SessionId)
{
    public string Initiator => nameof(DialAgentApi);
}
