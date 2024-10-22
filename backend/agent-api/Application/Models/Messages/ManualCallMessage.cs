namespace KL.Agent.API.Application.Models.Messages;

public record ManualCallMessage(
    long ClientId,
    long AgentId,
    string Phone)
{
    public string Initiator => nameof(DialAgentApi);
}
