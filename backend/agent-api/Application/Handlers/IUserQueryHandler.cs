using KL.Agent.API.Application.Models.Responses;

namespace KL.Agent.API.Application.Handlers;

public interface IUserQueryHandler
{
    Task<MeResponse?> Handle(long clientId, long userId, CancellationToken ct = default);
}