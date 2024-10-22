using KL.Agent.API.Application.Enums;

namespace KL.Agent.API.Application.Handlers;

public interface IAgentCurrentStatusHandler
{
    Task Handle(long agentId, AgentStatusTypes status, CancellationToken ct = default);
}
