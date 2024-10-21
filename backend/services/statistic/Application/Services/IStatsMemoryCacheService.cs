using Plat4Me.Dial.Statistic.Api.Application.Cache;
using Plat4Me.Dial.Statistic.Api.Application.Models.Messages;
using Plat4Me.Dial.Statistic.Api.Application.Models.StatisticCache;

namespace Plat4Me.Dial.Statistic.Api.Application.Services;

public interface IStatsMemoryCacheService : ICacheService<CdrStatisticCache>
{
    Task<CdrStatisticCache?> GetCallDetailRecords(long clientId, CancellationToken ct = default);
    Task<IEnumerable<CdrUpdatedMessage>?> FilterByPeriod(long clientId, DateTimeOffset from, DateTimeOffset to, CancellationToken ct = default);
}