using Plat4Me.DialClientApi.Application.Models.Responses;

namespace Plat4Me.DialClientApi.Application.Handlers;

public interface IUserQueryHandler
{
    Task<MeResponse?> Handle(long clientId, long userId, CancellationToken ct = default);
}