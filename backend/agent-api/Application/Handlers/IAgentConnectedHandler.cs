namespace KL.Agent.API.Application.Handlers;

public interface IAgentConnectedHandler
{
    Task Handle(long clientId, long agentId);
}
