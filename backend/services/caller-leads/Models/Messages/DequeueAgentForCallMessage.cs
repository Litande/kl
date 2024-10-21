namespace Plat4Me.DialLeadCaller.Application.Models.Messages;

public record DequeueAgentForCallMessage(
    long ClientId,
    long AgentId,
    string Initiator);
