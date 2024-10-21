namespace Plat4Me.DialLeadCaller.Application.Models.Messages;

public record ManualCallMessage(
    long ClientId,
    long AgentId,
    string Phone,
    string Initiator);
