using Plat4Me.DialAgentApi.Application.Models.Responses;

namespace Plat4Me.DialAgentApi.Application.Handlers;

public interface ICallAgainHandler
{
    Task<HubResponse> Handle(long clientId, long agentId);
}
