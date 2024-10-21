using Plat4Me.DialClientApi.Application.Extensions;
using Plat4Me.DialClientApi.Application.Models.Requests;
using Plat4Me.DialClientApi.Application.Models.Responses;
using Plat4Me.DialClientApi.Application.Models.Responses.Leads;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialClientApi.Application.Handlers.Leads;

public class LeadsQueueQueryHandler : ILeadsQueueQueryHandler
{
    private readonly ILeadQueueRepository _leadQueueRepository;
    private readonly IQueueLeadsCacheRepository _queueLeadsCacheRepository;

    public LeadsQueueQueryHandler(
        ILeadQueueRepository leadQueueRepository,
        IQueueLeadsCacheRepository queueLeadsCacheRepository)
    {
        _leadQueueRepository = leadQueueRepository;
        _queueLeadsCacheRepository = queueLeadsCacheRepository;
    }

    public async Task<IReadOnlyCollection<LeadQueueItem>> Handle(
        long clientId,
        CancellationToken ct = default)
    {
        var leadQueues = await _leadQueueRepository.GetEnabledQueuesWithAgents(clientId, ct);
        var queueLeadsCaches = await _queueLeadsCacheRepository.GetAll(clientId, ct);

        var result = leadQueues.ToLeadQueuesResponse(queueLeadsCaches);
        return result;
    }

    public async Task<PaginatedResponse<LeadItem>?> Handle(
        long clientId,
        long queueId,
        PaginationRequest pagination,
        CancellationToken ct = default)
    {
        return await _queueLeadsCacheRepository.GetLeads(clientId, queueId, pagination, ct);
    }
}
