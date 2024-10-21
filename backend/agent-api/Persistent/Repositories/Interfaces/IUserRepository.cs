using Plat4Me.DialAgentApi.Persistent.Entities;

namespace Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetById(long clientId, long userId, CancellationToken ct = default);
    Task SetOfflineSince(long clientId, long userId, DateTimeOffset? offlineSince, CancellationToken ct = default);
}
