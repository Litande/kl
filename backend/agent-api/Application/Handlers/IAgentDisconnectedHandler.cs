namespace Plat4Me.DialAgentApi.Application.Handlers;

public interface IAgentDisconnectedHandler
{
    Task Handle(long clientId, long agentId);
    Task Handle();
}
