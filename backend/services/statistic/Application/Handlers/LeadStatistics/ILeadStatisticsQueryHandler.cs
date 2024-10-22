using KL.Statistics.Application.Models.StatisticCache;

namespace KL.Statistics.Application.Handlers.LeadStatistics;

public interface ILeadStatisticsQueryHandler
{
    Task<IReadOnlyCollection<LeadStatisticCache>> Handle(long clientId, CancellationToken ct = default);
}