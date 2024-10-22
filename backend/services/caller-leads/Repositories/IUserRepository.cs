using KL.Caller.Leads.Models;
using KL.Caller.Leads.Models.Entities;

namespace KL.Caller.Leads.Repositories;

public interface IUserRepository
{
    Task<IDictionary<long, AgentScore>> GetAgentsWithScore(long clientId, CancellationToken ct = default);
    Task<User?> Get(long clientId, long userId, CancellationToken ct = default);
}
