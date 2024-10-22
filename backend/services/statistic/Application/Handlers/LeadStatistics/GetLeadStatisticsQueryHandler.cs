using KL.Statistics.Application.Models.StatisticCache;
using KL.Statistics.DAL.Repositories;

namespace KL.Statistics.Application.Handlers.LeadStatistics;

public class GetLeadStatisticsQueryHandler : ILeadStatisticsQueryHandler
{
    private readonly ILeadStatisticCacheRepository _statisticCacheRepository;

    public GetLeadStatisticsQueryHandler(ILeadStatisticCacheRepository statisticCacheRepository)
    {
        _statisticCacheRepository = statisticCacheRepository;
    }

    public async Task<IReadOnlyCollection<LeadStatisticCache>> Handle(long clientId, CancellationToken ct = default)
    {
        var statistics = await _statisticCacheRepository.GetLeadStatisticByClient(clientId);

        return statistics?.Statistics ?? new List<LeadStatisticCache>();
    }
}
