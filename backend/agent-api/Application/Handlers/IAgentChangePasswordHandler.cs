using KL.Agent.API.Application.Models.Requests;

namespace KL.Agent.API.Application.Handlers;

public interface IAgentChangePasswordHandler
{
    Task Handle(long agentId, AgentChangePasswordRequest request,  CancellationToken ct = default);
}
