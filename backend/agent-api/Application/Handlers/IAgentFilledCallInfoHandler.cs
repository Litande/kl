using Plat4Me.DialAgentApi.Application.Models.Requests;

namespace Plat4Me.DialAgentApi.Application.Handlers;

public interface IAgentFilledCallInfoHandler
{
    Task Handle(long clientId, long agentId, AgentFilledCallRequest request, bool isGenerated = false, CancellationToken ct = default);
}
