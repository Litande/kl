namespace KL.Agent.API.Application.Handlers;

public interface IAgentDisconnectedHandler
{
    Task Handle(long clientId, long agentId);
    Task Handle();
}
