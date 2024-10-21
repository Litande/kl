using Plat4Me.DialLeadCaller.Application.Models.Requests;

namespace Plat4Me.DialLeadCaller.Application.Handlers;

public interface IAgentFilledCallInfoHandler
{
    Task Handle(long clientId, long agentId, AgentFilledCallRequest request, bool isGenerated = false, CancellationToken ct = default);
}
