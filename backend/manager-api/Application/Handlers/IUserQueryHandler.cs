using KL.Manager.API.Application.Models.Responses;

namespace KL.Manager.API.Application.Handlers;

public interface IUserQueryHandler
{
    Task<MeResponse?> Handle(long clientId, long userId, CancellationToken ct = default);
}