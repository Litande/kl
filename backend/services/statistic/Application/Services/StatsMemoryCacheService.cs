using KL.Statistics.Application.Cache;
using KL.Statistics.Application.Common;
using KL.Statistics.Application.Common.Enums;
using KL.Statistics.Application.Common.Extensions;
using KL.Statistics.Application.Models.Messages;
using KL.Statistics.Application.Models.StatisticCache;
using KL.Statistics.DAL.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace KL.Statistics.Application.Services;

public class StatsMemoryCacheService : MemoryCacheService<CdrStatisticCache>, IStatsMemoryCacheService
{
    private readonly IStatisticPeriodService _periodService;
    private readonly ICdrRepository _cdrRepository;

    public StatsMemoryCacheService(
        IMemoryCache memoryCache,
        ICdrRepository cdrRepository,
        IStatisticPeriodService periodService) : base(memoryCache)
    {
        _cdrRepository = cdrRepository;
        _periodService = periodService;
    }

    public async Task<CdrStatisticCache?> GetCallDetailRecords(
        long clientId,
        CancellationToken ct = default)
    {
        var key = CacheKeys.CdrKey + clientId;
        RemoveCacheIfInsertedDayExpired(key);
        var statsModel = Get(key);

        if (statsModel is not null)
            return statsModel;

        var cachePeriod = GetCachePeriod();
        var callRecords = await _cdrRepository.GetCallsByPeriod(clientId, cachePeriod.From, cachePeriod.To, ct);
        statsModel = new CdrStatisticCache(callRecords.ToCdrChangedMessage(), DateTime.UtcNow);

        Set(key, statsModel);

        return statsModel;
    }

    private void RemoveCacheIfInsertedDayExpired(string key)
    {
        var statsModel = Get(key);
        if (statsModel is null || statsModel.InsertedDate.Date == DateTime.UtcNow.Date) return;
        Remove(key);
    }


    public async Task<IEnumerable<CdrUpdatedMessage>?> FilterByPeriod(
        long clientId,
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken ct = default)
    {
        var statsModel = await GetCallDetailRecords(clientId, ct);

        var result = statsModel?.CallDetailRecords.Where(c => c.OriginatedAt > from && c.OriginatedAt <= to);
        return result ?? Enumerable.Empty<CdrUpdatedMessage>();
    }


    private (DateTimeOffset From, DateTimeOffset To) GetCachePeriod()
    {
        var currentPeriod = _periodService.GetCurrent(PeriodTypes.Month);
        var period = currentPeriod.To - currentPeriod.From;
        var prevPeriodStart = currentPeriod.From - period;

        return (From: prevPeriodStart, currentPeriod.To);
    }
}