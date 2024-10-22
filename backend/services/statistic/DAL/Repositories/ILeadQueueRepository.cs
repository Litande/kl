using KL.Statistics.Application.Models.Entities;

namespace KL.Statistics.DAL.Repositories;

public interface ILeadQueueRepository
{
    Task<IReadOnlyCollection<LeadQueue>> GetEnabledQueuesByAgents(long clientId, IEnumerable<long> agentIds, CancellationToken ct = default);
}
