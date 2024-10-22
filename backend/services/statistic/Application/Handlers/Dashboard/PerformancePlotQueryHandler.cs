using KL.Statistics.Application.Common.Enums;
using KL.Statistics.Application.Models.Responses;
using KL.Statistics.Application.Services;

namespace KL.Statistics.Application.Handlers.Dashboard;

public class PerformancePlotQueryHandler : IPerformancePlotQueryHandler
{
    private readonly IStatsMemoryCacheService _statsMemoryCacheService;

    public PerformancePlotQueryHandler(IStatsMemoryCacheService statsMemoryCacheService)
    {
        _statsMemoryCacheService = statsMemoryCacheService;
    }

    public async Task<PerformancePlotData> Handle(
        long clientId,
        PerformanceTypes type,
        DateTimeOffset from,
        DateTimeOffset to,
        int step,
        CancellationToken ct = default)
    {
        if (from >= to)
            throw new ArgumentException("Invalid from/to");

        if (step == 0) step = 1;
        var calls = await _statsMemoryCacheService.FilterByPeriod(clientId, from, to, ct);

        var target = ((type switch
        {
            PerformanceTypes.AvgCall => calls,
            PerformanceTypes.AvgNewClient => calls?.Where(x => x.LeadStatusAfter is LeadStatusTypes.InTheMoney),
            PerformanceTypes.SuccessCall => calls?.Where(x => x.LeadStatusAfter
                is LeadStatusTypes.CallAgainGeneral
                or LeadStatusTypes.CallAgainPersonal
                or LeadStatusTypes.InTheMoney),
            _ => throw new ArgumentException("Invalid PerformanceType")
        })!).OrderBy(x => x.OriginatedAt);

        var shift = TimeSpan.FromMinutes(step);
        var cursor = from + shift;
        List<PerformancePlotDataItem> values = new();

        using var enumerator = target.GetEnumerator();
        var canMove = enumerator.MoveNext();
        while (cursor <= to)
        {
            var value = 0;

            while (canMove && !(enumerator.Current!.OriginatedAt >= cursor))
            {
                ++value;
                canMove = enumerator.MoveNext();
            }

            values.Add(new PerformancePlotDataItem(cursor, value));
            cursor += shift;
        }

        return new PerformancePlotData(type, from, to, step, values.ToArray());
    }
}