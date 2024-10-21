using Plat4Me.DialLeadCaller.Application.Models;
using Plat4Me.DialLeadCaller.Application.Models.Entities;

namespace Plat4Me.DialLeadCaller.Application.Repositories;

public interface IUserRepository
{
    Task<IDictionary<long, AgentScore>> GetAgentsWithScore(long clientId, CancellationToken ct = default);
    Task<User?> Get(long clientId, long userId, CancellationToken ct = default);
}
