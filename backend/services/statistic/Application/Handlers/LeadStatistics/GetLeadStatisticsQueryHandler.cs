using Plat4Me.Dial.Statistic.Api.Application.Models.StatisticCache;
using Plat4Me.Dial.Statistic.Api.DAL.Repositories;

namespace Plat4Me.Dial.Statistic.Api.Application.Handlers.LeadStatistics;

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
