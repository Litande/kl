using Plat4Me.DialAgentApi.Application.Models.Requests;

namespace Plat4Me.DialAgentApi.Application.Handlers;

public interface IAgentChangePasswordHandler
{
    Task Handle(long agentId, AgentChangePasswordRequest request,  CancellationToken ct = default);
}
