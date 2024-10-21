using Plat4Me.Dial.Statistic.Api.Application.Models.Entities;

namespace Plat4Me.Dial.Statistic.Api.DAL.Repositories;

public interface ILeadQueueRepository
{
    Task<IReadOnlyCollection<LeadQueue>> GetEnabledQueuesByAgents(long clientId, IEnumerable<long> agentIds, CancellationToken ct = default);
}
