namespace Plat4Me.DialAgentApi.Application.Models.Messages;

public record CallAgainMessage(
    long ClientId,
    long AgentId,
    string SessionId)
{
    public string Initiator => nameof(DialAgentApi);
}
