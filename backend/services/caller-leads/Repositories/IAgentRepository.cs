using KL.Caller.Leads.Models.Entities;

namespace KL.Caller.Leads.Repositories;

public interface IAgentRepository
{
    Task<IReadOnlyCollection<User>> GetAll(long clientId, CancellationToken ct = default);
    Task<User?> GetById(long clientId, long agentId, CancellationToken ct = default);
}