using KL.Caller.Leads.Models.Requests;

namespace KL.Caller.Leads.Handlers.Contracts;

public interface IAgentFilledCallInfoHandler
{
    Task Handle(long clientId, long agentId, AgentFilledCallRequest request, bool isGenerated = false, CancellationToken ct = default);
}
