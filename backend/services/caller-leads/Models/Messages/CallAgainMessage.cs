namespace Plat4Me.DialLeadCaller.Application.Models.Messages;

public record CallAgainMessage(
    long ClientId,
    long AgentId,
    string SessionId,
    string Initiator);
