using KL.Agent.API.Application.Enums;
using KL.Agent.API.Application.Models.Responses;

namespace KL.Agent.API.Application.Handlers;

public interface IAgentChangeStatusRequestHandler
{
    Task<HubResponse> Handle(long clientId, long agentId, AgentStatusTypes status);
}
