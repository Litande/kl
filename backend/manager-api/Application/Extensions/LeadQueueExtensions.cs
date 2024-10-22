using KL.Manager.API.Application.Enums;
using KL.Manager.API.Application.Models.Requests;
using KL.Manager.API.Application.Models.Responses;
using KL.Manager.API.Application.Models.Responses.LeadGroups;
using KL.Manager.API.Application.Models.Responses.LeadQueues;
using KL.Manager.API.Application.Models.Responses.Leads;
using KL.Manager.API.Persistent.Entities;
using KL.Manager.API.Persistent.Entities.Cache;

namespace KL.Manager.API.Application.Extensions;

public static class LeadQueueExtensions
{
    public static IReadOnlyCollection<LeadGroup> ToLeadGroupsResponse(
        this IEnumerable<LeadQueue> queueEntities,
        IEnumerable<QueueLeadCache> queueLeadsCaches,
        IEnumerable<long> onlineAgentIds,
        IDictionary<long, QueueDropRateCache> queueDropRateCaches)
    {
        LeadGroup Mapper(LeadQueue queue)
        {
            var agentsCount = queue.Agents.Count(r => onlineAgentIds.Contains(r.Id));
            var leadsCount = queueLeadsCaches.Count(i => i.QueueId == queue.Id);
            queueDropRateCaches.TryGetValue(queue.Id, out var queueCache);

            return new LeadGroup(
                Name: queue.Name,
                Agents: agentsCount,
                Ratio: queue.Ratio,
                DropRatePercentage: (queueCache?.DropRate ?? 0).ToString("P2"),
                LeadsCount: leadsCount,
                LeadQueueId: queue.Id,
                queue.QueueType,
                queue.DropRateUpperThreshold,
                queue.DropRateLowerThreshold,
                queue.DropRatePeriod,
                queue.RatioStep,
                queue.RatioFreezeTime,
                queue.MaxRatio,
                queue.MinRatio
            );
        }

        return queueEntities
            .Select(Mapper)
            .ToArray();
    }

    public static IReadOnlyCollection<LeadGroup> ToLeadGroupsResponse(
        this LeadQueue queueEntity,
        IEnumerable<QueueLeadCache> queueLeadsCaches,
        IEnumerable<long> onlineAgentIds,
        IDictionary<long, QueueDropRateCache> queueDropRateCaches)
    {
        LeadGroup Mapper(LeadQueue queue)
        {
            var agentsCount = queue.Agents.Count(r => onlineAgentIds.Contains(r.Id));
            var leadsCount = queueLeadsCaches.Count(i => i.QueueId == queue.Id);
            queueDropRateCaches.TryGetValue(queue.Id, out var queueCache);

            return new LeadGroup(
                Name: queue.Name,
                Agents: agentsCount,
                Ratio: queue.Ratio,
                DropRatePercentage: (queueCache?.DropRate ?? 0).ToString("P2"),
                LeadsCount: leadsCount,
                LeadQueueId: queue.Id,
                queue.QueueType,
                queue.DropRateUpperThreshold,
                queue.DropRateLowerThreshold,
                queue.DropRatePeriod,
                queue.RatioStep,
                queue.RatioFreezeTime,
                queue.MaxRatio,
                queue.MinRatio
            );
        }

        return new[] {Mapper(queueEntity)};
    }

    public static IReadOnlyCollection<LeadQueueItem> ToLeadQueuesResponse(
        this IEnumerable<LeadQueue> queueEntities,
        IEnumerable<QueueLeadCache>? queueLeadsCaches)
    {
        var queueUpdatesDic = queueLeadsCaches?
            .GroupBy(x => x.QueueId)
            .ToDictionary(x => x.Key);

        LeadQueueItem Mapper(LeadQueue queue)
        {
            if (queueUpdatesDic is null || !queueUpdatesDic.ContainsKey(queue.Id))
                return new LeadQueueItem(queue.Name, LeadsCount: 0, Leads: Array.Empty<LeadItem>());

            var leads = GetSortedLeads(queue, queueUpdatesDic[queue.Id]).ToArray();

            return new LeadQueueItem(GroupName: queue.Name, LeadsCount: leads.Length, Leads: leads);
        }

        return queueEntities
            .Select(Mapper)
            .ToArray();
    }

    public static LeadQueueItemResponse ToResponse(this LeadQueue entity) =>
        new(entity.Id, entity.Name);

    private static IEnumerable<LeadItem> GetSortedLeads(LeadQueue queue, IEnumerable<QueueLeadCache> queueLeads)
    {
        var leads = queueLeads
            .OrderByDescending(x => x.SystemStatus.HasValue);

        if (queue.QueueType is LeadQueueTypes.Future)
            leads = leads.ThenBy(x => x.RemindOn);

        return leads
            .ThenByDescending(x => x.Score)
            .Select(i => new LeadItem(i.LeadId, i.Score, i.SystemStatus, i.Status));
    }

    public static PaginatedResponse<LeadItem> ToLeadsPaginateResponse(
        this ICollection<QueueLeadCache> query,
        PaginationRequest pagination)
    {
        var totalCount = query.Count;

        var items = totalCount > 0
            ? ApplyPaginationForQueueLeads(query, pagination)
                .ToArray()
            : Enumerable.Empty<LeadItem>();

        return new PaginatedResponse<LeadItem>
        {
            TotalCount = totalCount,
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            Items = items,
        };
    }

    private static IEnumerable<LeadItem> ApplyPaginationForQueueLeads(
        IEnumerable<QueueLeadCache> query,
        PaginationRequest pagination)
    {
        query = query.OrderByDescending(x => x.SystemStatus.HasValue)
            .ThenByDescending(x => x.Score).ToArray();

        query = query
            .Skip(pagination.PageSize * (pagination.Page - 1))
            .Take(pagination.PageSize);

        var q = query.Select(x => new LeadItem(x.LeadId, x.Score, x.SystemStatus, x.Status));
        return q;
    }
}
