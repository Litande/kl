using KL.Manager.API.Application.Models.Requests;
using KL.Manager.API.Application.Models.Responses;
using KL.Manager.API.Application.Models.Responses.Leads;
using KL.Manager.API.Persistent.Entities.Cache;

namespace KL.Manager.API.Persistent.Repositories.Interfaces;

public interface IQueueLeadsCacheRepository
{
    Task<IReadOnlyCollection<QueueLeadCache>> GetQueue(long clientId, long queueId, CancellationToken ct = default);
    Task<PaginatedResponse<LeadItem>> GetLeads(long clientId, long queueId, PaginationRequest pagination, CancellationToken ct = default);
    Task<IReadOnlyCollection<QueueLeadCache>> GetAll(long clientId, CancellationToken ct = default);
    Task<QueueLeadCache?> Get(long leadId);
}
