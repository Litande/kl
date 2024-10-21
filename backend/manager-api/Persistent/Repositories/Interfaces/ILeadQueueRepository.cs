using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Application.Models.Requests.LeadQueue;
using Plat4Me.DialClientApi.Application.Models.Responses.LeadQueues;
using Plat4Me.DialClientApi.Persistent.Entities;

namespace Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

public interface ILeadQueueRepository
{
    Task<LeadQueuesResponse> GetAll(long clientId, LeadQueueStatusTypes? status = LeadQueueStatusTypes.Enable, CancellationToken ct = default);
    Task<IReadOnlyCollection<LeadQueue>> GetEnabledQueuesWithAgents(long clientId, CancellationToken ct = default);
    Task<IReadOnlyCollection<LeadQueue>> GetEnabledQueuesByAgents(long clientId, IEnumerable<long> agentIds, CancellationToken ct = default);
    Task<LeadQueue?> GetEnabledQueueByAgent(long clientId, long agentId, CancellationToken ct = default);
    Task<IReadOnlyCollection<LeadQueue>> GetEnabledQueues(long clientId, IReadOnlyCollection<long> queueIds, CancellationToken ct = default);
    Task<LeadQueue?> GetEnabledQueue(long clientId, long queueId, CancellationToken ct = default);
    Task<LeadQueue?> UpdateRatio(long clientId, long queueId, double ratio, CancellationToken ct = default);
    Task UpdateLeadQueue(long clientId, long leadQueueId, UpdateLeadQueueRequest request, CancellationToken ct = default);
}
