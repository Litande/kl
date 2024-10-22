using KL.Statistics.Application.Models;
using KL.Statistics.Application.Models.Entities;
using KL.Statistics.Application.Models.Responses;

namespace KL.Statistics.Application.Common.Extensions;

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
}