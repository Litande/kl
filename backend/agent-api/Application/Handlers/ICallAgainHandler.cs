using KL.Agent.API.Application.Models.Responses;

namespace KL.Agent.API.Application.Handlers;

public interface ICallAgainHandler
{
    Task<HubResponse> Handle(long clientId, long agentId);
}
