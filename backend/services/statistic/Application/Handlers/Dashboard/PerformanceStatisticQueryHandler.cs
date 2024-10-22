using KL.Statistics.Application.Common.Enums;
using KL.Statistics.Application.Models.Messages;
using KL.Statistics.Application.Models.Responses;
using KL.Statistics.Application.Services;

namespace KL.Statistics.Application.Handlers.Dashboard;

public class PerformanceStatisticQueryHandler : IPerformanceStatisticQueryHandler
{
    private readonly IStatsMemoryCacheService _statsMemoryCacheService;

    public PerformanceStatisticQueryHandler(IStatsMemoryCacheService statsMemoryCacheService)
    {
        _statsMemoryCacheService = statsMemoryCacheService;
    }

    public async Task<PerformanceStatisticsData> Handle(
        long clientId,
        PerformanceTypes type,
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken ct)
    {
        if (from >= to)
            throw new ArgumentException("Invalid from/to");

        var period = to - from;
        var prevPeriodStart = from - period;
        var calls = await _statsMemoryCacheService.FilterByPeriod(clientId, prevPeriodStart, to, ct);
        var callsTargetPeriod = calls.Where(x => x.OriginatedAt >= from).ToArray();
        var callsPrevPeriod = calls.Where(x => x.OriginatedAt < from).ToArray();

        return GetPerformanceStatistics(type, callsTargetPeriod, callsPrevPeriod, from, to);
    }

    public async Task<IReadOnlyCollection<PerformanceStatisticsData>> Handle(
        long clientId,
        PerformanceTypes[] types,
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken ct)
    {
        if (from >= to)
            throw new ArgumentException("Invalid from/to");
        var period = to - from;
        var prevPeriodStart = from - period;
        var calls = await _statsMemoryCacheService.FilterByPeriod(clientId, prevPeriodStart, to, ct);
        var callsTargetPeriod = calls.Where(x => x.OriginatedAt >= from).ToArray();
        var callsPrevPeriod = calls.Where(x => x.OriginatedAt < from).ToArray();

        var list = new List<PerformanceStatisticsData>(types.Length);

        foreach (var type in types)
        {
            var statistic = GetPerformanceStatistics(type, callsTargetPeriod, callsPrevPeriod, from, to);
            list.Add(statistic);
        }

        return list;
    }

    private static PerformanceStatisticsData GetPerformanceStatistics(
        PerformanceTypes type,
        CdrUpdatedMessage[] callsTargetPeriod,
        CdrUpdatedMessage[] callsPreviousPeriod,
        DateTimeOffset from,
        DateTimeOffset to)
    {
        return type switch
        {
            PerformanceTypes.SuccessCall => CalculateSuccessCalls(callsTargetPeriod, callsPreviousPeriod, from, to),
            PerformanceTypes.AvgCall => CalculateAverageCalls(callsTargetPeriod, callsPreviousPeriod, from, to),
            PerformanceTypes.AvgNewClient => CalculateAverageNewClients(callsTargetPeriod, callsPreviousPeriod, from, to),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, $"Wrong {nameof(PerformanceTypes)} type")
        };
    }

    private static PerformanceStatisticsData CalculateAverageCalls(
        CdrUpdatedMessage[] callsTargetPeriod,
        CdrUpdatedMessage[] callsPreviousPeriod,
        DateTimeOffset from,
        DateTimeOffset to)
    {
        var countAgents = callsTargetPeriod.Select(x => x.UserId).Distinct().Count();
        var countPreviousAgents = callsPreviousPeriod.Select(x => x.UserId).Distinct().Count();
        var value = countAgents == 0 ? 0 :
            (long)Math.Round(callsTargetPeriod.LongCount() / (double)countAgents);
        var previousValue = countPreviousAgents == 0 ? 0 :
            (long)Math.Round(callsPreviousPeriod.LongCount() / (double)countPreviousAgents);
        return new PerformanceStatisticsData(PerformanceTypes.AvgCall, from, to, value, value - previousValue);
    }

    private static PerformanceStatisticsData CalculateAverageNewClients(
        CdrUpdatedMessage[] callsTargetPeriod,
        CdrUpdatedMessage[] callsPreviousPeriod,
        DateTimeOffset from,
        DateTimeOffset to)
    {
        var countAgents = callsTargetPeriod.Select(x => x.UserId).Distinct().Count();
        var countPreviousAgents = callsPreviousPeriod.Select(x => x.UserId).Distinct().Count();
        var value = countAgents == 0 ? 0 :
            (long)Math.Round(callsTargetPeriod.LongCount(x => x.LeadStatusAfter is LeadStatusTypes.InTheMoney) / (double)countAgents);
        var previousValue = countPreviousAgents == 0 ? 0 :
            (long)Math.Round(callsPreviousPeriod.LongCount(x => x.LeadStatusAfter is LeadStatusTypes.InTheMoney) / (double)countPreviousAgents);

        return new PerformanceStatisticsData(PerformanceTypes.AvgNewClient, from, to, value, value - previousValue);
    }

    private static PerformanceStatisticsData CalculateSuccessCalls(
        CdrUpdatedMessage[] callsTargetPeriod,
        CdrUpdatedMessage[] callsPreviousPeriod,
        DateTimeOffset from,
        DateTimeOffset to)
    {
        var value = callsTargetPeriod.LongCount(x =>
            x.LeadStatusAfter is LeadStatusTypes.CallAgainGeneral
                or LeadStatusTypes.CallAgainPersonal
                or LeadStatusTypes.InTheMoney);

        var previousValue = callsPreviousPeriod.LongCount(x =>
            x.LeadStatusAfter is LeadStatusTypes.CallAgainGeneral
                or LeadStatusTypes.CallAgainPersonal
                or LeadStatusTypes.InTheMoney);

        return new PerformanceStatisticsData(PerformanceTypes.SuccessCall, from, to, value, value - previousValue);
    }
}