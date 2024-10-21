using Plat4Me.DialLeadCaller.Application.Models;
using Plat4Me.DialLeadCaller.Application.Models.Entities;

namespace Plat4Me.DialLeadCaller.Application.Repositories;

public interface ILeadQueueRepository
{
    Task<IReadOnlyCollection<LeadQueue>> GetAll(CancellationToken ct = default);
    Task<IReadOnlyCollection<LeadQueueAgents>> GetAllWithAgentsOrdered(CancellationToken ct = default);
    Task<LeadQueueAgents?> GetWithAgents(long queueId, CancellationToken ct = default);
    Task UpdateRatio(IDictionary<long, double> queueRatioUpdates, CancellationToken ct = default);
}
