namespace KL.Caller.Leads.Models.Messages;

public record CallAgainMessage(
    long ClientId,
    long AgentId,
    string SessionId,
    string Initiator);
