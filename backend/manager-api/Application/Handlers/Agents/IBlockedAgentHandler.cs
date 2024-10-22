namespace KL.Manager.API.Application.Handlers.Agents;

public interface IBlockedAgentHandler
{
    Task Handle(long agentId, CancellationToken ct = default);
}