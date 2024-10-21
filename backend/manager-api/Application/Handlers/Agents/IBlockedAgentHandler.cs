namespace Plat4Me.DialClientApi.Application.Handlers.Agents;

public interface IBlockedAgentHandler
{
    Task Handle(long agentId, CancellationToken ct = default);
}