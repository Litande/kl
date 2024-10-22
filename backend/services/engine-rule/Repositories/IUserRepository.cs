using KL.Engine.Rule.Models.Entities;

namespace KL.Engine.Rule.Repositories;

public interface IUserRepository
{
    Task<IReadOnlyCollection<User>> GetOfflineAgentsSince(DateTimeOffset fromDate, CancellationToken ct = default);
}
