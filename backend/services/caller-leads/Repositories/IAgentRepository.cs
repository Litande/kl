using Plat4Me.DialLeadCaller.Application.Models.Entities;

namespace Plat4Me.DialLeadCaller.Application.Repositories;

public interface IAgentRepository
{
    Task<IReadOnlyCollection<User>> GetAll(long clientId, CancellationToken ct = default);
    Task<User?> GetById(long clientId, long agentId, CancellationToken ct = default);
}