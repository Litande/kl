namespace KL.Agent.API.Application.Models.Messages;

public record AgentReplacedMessage(
    long ClientId,
    long AgentId,
    string SessionId,
    long SipProviderId);
