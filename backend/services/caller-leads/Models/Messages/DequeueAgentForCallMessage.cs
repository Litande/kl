namespace KL.Caller.Leads.Models.Messages;

public record DequeueAgentForCallMessage(
    long ClientId,
    long AgentId,
    string Initiator);
