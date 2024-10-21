using Plat4Me.DialAgentApi.Application.Models.Responses;

namespace Plat4Me.DialAgentApi.Application.Handlers;

public interface IUserQueryHandler
{
    Task<MeResponse?> Handle(long clientId, long userId, CancellationToken ct = default);
}