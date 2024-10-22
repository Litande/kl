namespace KL.Caller.Leads.Models.Messages;

public record ManualCallMessage(
    long ClientId,
    long AgentId,
    string Phone,
    string Initiator);
