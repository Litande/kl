using KL.Manager.API.Application.Extensions;
using KL.Manager.API.Application.Models.Responses.Leads;
using KL.Manager.API.Persistent.Entities;
using KL.Manager.API.Persistent.Repositories.Interfaces;

namespace KL.Manager.API.Application.Handlers.Leads;

public class LeadInfoQueryHandler : ILeadInfoQueryHandler
{
    private readonly ILeadRepository _leadRepository;
    private readonly ICallInfoCacheRepository _callInfoCacheRepository;
    private readonly IQueueLeadsCacheRepository _queueLeadsCache;
    private readonly ILeadQueueRepository _leadQueueRepository;

    public LeadInfoQueryHandler(
        ILeadRepository leadRepository,
        ICallInfoCacheRepository callInfoCacheRepository,
        IQueueLeadsCacheRepository queueLeadsCache,
        ILeadQueueRepository leadQueueRepository)
    {
        _leadRepository = leadRepository;
        _callInfoCacheRepository = callInfoCacheRepository;
        _queueLeadsCache = queueLeadsCache;
        _leadQueueRepository = leadQueueRepository;
    }

    public async Task<LeadShortInfo> Handle(
        long clientId,
        long leadId,
        CancellationToken ct = default)
    {
        var cachedInfo = await _callInfoCacheRepository.GetAgentByLead(clientId, leadId);
        LeadQueue? leadQueue = null;

        var cachedLeadQueue = (await _queueLeadsCache
            .GetAll(clientId, ct)).FirstOrDefault(x => x.LeadId == leadId);

        if (cachedLeadQueue is not null)
        {
            leadQueue = (await _leadQueueRepository
                    .GetEnabledQueues(clientId, new[] { cachedLeadQueue.QueueId }, ct))
                .FirstOrDefault();
        }

        var lead = await _leadRepository.GetByIdAsNoTracking(clientId, leadId, ct);
        if (lead is null)
            throw new KeyNotFoundException($"Cannot find lead with id: {leadId}");

        return lead.ToLeadShortInfoResponse(cachedInfo, leadQueue);
    }
}
