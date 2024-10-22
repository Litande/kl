using KL.Agent.API.Application.Models.Responses;

namespace KL.Agent.API.Application.Handlers;

public interface IManualCallHandler
{
    Task<HubResponse> Handle(long clientId, long agentId, string phone, CancellationToken ct = default);
}
