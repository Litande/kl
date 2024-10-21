namespace Plat4Me.DialAgentApi.Application.Models.Messages;

public record AgentReplacedMessage(
    long ClientId,
    long AgentId,
    string SessionId,
    long SipProviderId);
