using KL.Manager.API.Application.Models.Requests.Agents;

namespace KL.Manager.API.Application.Handlers.Agents;

public interface IAgentChangePasswordHandler
{
    Task Handle(long agentId, AgentChangePasswordRequest request,  CancellationToken ct = default);
}
