using Plat4Me.Dial.Statistic.Api.Application.Models.StatisticCache;

namespace Plat4Me.Dial.Statistic.Api.Application.Handlers.LeadStatistics;

public interface ILeadStatisticsQueryHandler
{
    Task<IReadOnlyCollection<LeadStatisticCache>> Handle(long clientId, CancellationToken ct = default);
}