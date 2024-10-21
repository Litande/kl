using Plat4Me.DialAgentApi.Application.Enums;

namespace Plat4Me.DialAgentApi.Application.Handlers;

public interface IAgentCurrentStatusHandler
{
    Task Handle(long agentId, AgentStatusTypes status, CancellationToken ct = default);
}
