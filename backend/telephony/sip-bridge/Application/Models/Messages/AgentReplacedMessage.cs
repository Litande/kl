namespace Plat4Me.DialSipBridge.Application.Models.Messages;

public record AgentReplacedMessage(
    long ClientId,
    long AgentId,
    string SessionId,
    string Initiator,
    long SipProviderId);
