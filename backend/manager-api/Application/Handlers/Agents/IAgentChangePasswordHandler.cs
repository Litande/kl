using Plat4Me.DialClientApi.Application.Models.Requests.Agents;

namespace Plat4Me.DialClientApi.Application.Handlers.Agents;

public interface IAgentChangePasswordHandler
{
    Task Handle(long agentId, AgentChangePasswordRequest request,  CancellationToken ct = default);
}
