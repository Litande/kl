using Plat4Me.DialRuleEngine.Application.Models.Entities;

namespace Plat4Me.DialRuleEngine.Application.Repositories;

public interface IUserRepository
{
    Task<IReadOnlyCollection<User>> GetOfflineAgentsSince(DateTimeOffset fromDate, CancellationToken ct = default);
}
