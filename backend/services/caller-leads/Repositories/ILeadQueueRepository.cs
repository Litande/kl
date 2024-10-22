using KL.Caller.Leads.Models;
using KL.Caller.Leads.Models.Entities;

namespace KL.Caller.Leads.Repositories;

public interface ILeadQueueRepository
{
    Task<IReadOnlyCollection<LeadQueue>> GetAll(CancellationToken ct = default);
    Task<IReadOnlyCollection<LeadQueueAgents>> GetAllWithAgentsOrdered(CancellationToken ct = default);
    Task<LeadQueueAgents?> GetWithAgents(long queueId, CancellationToken ct = default);
    Task UpdateRatio(IDictionary<long, double> queueRatioUpdates, CancellationToken ct = default);
}
