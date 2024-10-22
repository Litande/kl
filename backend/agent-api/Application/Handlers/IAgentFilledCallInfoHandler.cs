using KL.Agent.API.Application.Models.Requests;

namespace KL.Agent.API.Application.Handlers;

public interface IAgentFilledCallInfoHandler
{
    Task Handle(long clientId, long agentId, AgentFilledCallRequest request, bool isGenerated = false, CancellationToken ct = default);
}
