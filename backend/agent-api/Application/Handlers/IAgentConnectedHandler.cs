namespace Plat4Me.DialAgentApi.Application.Handlers;

public interface IAgentConnectedHandler
{
    Task Handle(long clientId, long agentId);
}
