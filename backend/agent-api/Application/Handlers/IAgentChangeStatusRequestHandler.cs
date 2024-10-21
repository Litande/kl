using Plat4Me.DialAgentApi.Application.Enums;
using Plat4Me.DialAgentApi.Application.Models.Responses;

namespace Plat4Me.DialAgentApi.Application.Handlers;

public interface IAgentChangeStatusRequestHandler
{
    Task<HubResponse> Handle(long clientId, long agentId, AgentStatusTypes status);
}
