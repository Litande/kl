using Plat4Me.DialClientApi.Application.Models.Requests;
using Plat4Me.DialClientApi.Application.Models.Responses;
using Plat4Me.DialClientApi.Application.Models.Responses.Leads;
using Plat4Me.DialClientApi.Persistent.Entities.Cache;

namespace Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

public interface IQueueLeadsCacheRepository
{
    Task<IReadOnlyCollection<QueueLeadCache>> GetQueue(long clientId, long queueId, CancellationToken ct = default);
    Task<PaginatedResponse<LeadItem>> GetLeads(long clientId, long queueId, PaginationRequest pagination, CancellationToken ct = default);
    Task<IReadOnlyCollection<QueueLeadCache>> GetAll(long clientId, CancellationToken ct = default);
    Task<QueueLeadCache?> Get(long leadId);
}
