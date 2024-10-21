namespace Plat4Me.DialLeadCaller.Application.Models.Messages;

public record AgentReplacedMessage(
    long ClientId,
    long AgentId,
    string SessionId,
    long SipProviderId)
{
    public string Initiator => nameof(DialLeadCaller);
}
