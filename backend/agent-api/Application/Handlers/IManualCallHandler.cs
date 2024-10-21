using Plat4Me.DialAgentApi.Application.Models.Responses;

namespace Plat4Me.DialAgentApi.Application.Handlers;

public interface IManualCallHandler
{
    Task<HubResponse> Handle(long clientId, long agentId, string phone, CancellationToken ct = default);
}
