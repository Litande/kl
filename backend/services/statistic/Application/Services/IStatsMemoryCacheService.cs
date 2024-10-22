using KL.Statistics.Application.Cache;
using KL.Statistics.Application.Models.Messages;
using KL.Statistics.Application.Models.StatisticCache;

namespace KL.Statistics.Application.Services;

public interface IStatsMemoryCacheService : ICacheService<CdrStatisticCache>
{
    Task<CdrStatisticCache?> GetCallDetailRecords(long clientId, CancellationToken ct = default);
    Task<IEnumerable<CdrUpdatedMessage>?> FilterByPeriod(long clientId, DateTimeOffset from, DateTimeOffset to, CancellationToken ct = default);
}