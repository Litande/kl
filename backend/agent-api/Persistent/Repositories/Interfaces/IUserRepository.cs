using KL.Agent.API.Persistent.Entities;

namespace KL.Agent.API.Persistent.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetById(long clientId, long userId, CancellationToken ct = default);
    Task SetOfflineSince(long clientId, long userId, DateTimeOffset? offlineSince, CancellationToken ct = default);
}
