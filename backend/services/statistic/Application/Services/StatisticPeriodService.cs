using Microsoft.Extensions.Internal;
using Plat4Me.Dial.Statistic.Api.Application.Common.Enums;
using Plat4Me.Dial.Statistic.Api.Application.Models;

namespace Plat4Me.Dial.Statistic.Api.Application.Services;

public class StatisticPeriodService : IStatisticPeriodService
{
    private const int NumberOfHoursPerDay = 24;
    private const int NumberOfDaysPerWeek = (int)PeriodTypes.Week;
    private const int NumberOfDaysPerMonth = (int)PeriodTypes.Month;
    private const int NumberOfDaysPerYear = (int)PeriodTypes.Year;

    private readonly ISystemClock _clock;

    public StatisticPeriodService(ISystemClock clock)
    {
        _clock = clock;
    }

    public ReportPeriod GetCurrent(PeriodTypes type)
    {
        var to = _clock.UtcNow;
        var from = CalcDateTime(type, to.DateTime);
        return new ReportPeriod(type, from.DateTime, to.DateTime);
    }

    public ReportPeriod GetPrevious(ReportPeriod period)
    {
        var to = CalcDateTime(period.Type, period.To);
        var from = CalcDateTime(period.Type, period.From.AddDays(-1));
        return new ReportPeriod(period.Type, from.DateTime, to.DateTime);
    }

    public IEnumerable<ReportPeriod> SplitToPeriod(ReportPeriod period)
    {
        return period.Type switch
        {
            PeriodTypes.Today => Enumerable.Range(0, NumberOfHoursPerDay)
                .Select(index =>
                    new ReportPeriod(period.Type, period.From.AddHours(index), period.From.AddHours(index + 1))),

            PeriodTypes.Week => Enumerable.Range(0, NumberOfDaysPerWeek)
                .Select(index =>
                    new ReportPeriod(period.Type, period.From.AddDays(index), period.From.AddDays(index + 1))),

            PeriodTypes.Month => Enumerable.Range(0, NumberOfDaysPerMonth)
                .Select(index =>
                    new ReportPeriod(period.Type, period.From.AddDays(index), period.From.AddDays(index + 1))),

            PeriodTypes.Year => Enumerable.Range(0, NumberOfDaysPerYear)
                .Select(index =>
                    new ReportPeriod(period.Type, period.From.AddDays(index), period.From.AddDays(index + 1))),

            _ => throw new ArgumentOutOfRangeException(nameof(period.Type), period.Type, "Period not implemented")
        };
    }

    private static DateTimeOffset CalcDateTime(PeriodTypes type, DateTimeOffset dateTime)
    {
        var dateTimeStart = new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, TimeSpan.Zero);
        return type switch
        {
            PeriodTypes.Today => dateTimeStart,
            PeriodTypes.Week => dateTimeStart.AddDays(-NumberOfDaysPerWeek + 1),
            PeriodTypes.Month => dateTimeStart.AddDays(-NumberOfDaysPerMonth + 1),
            PeriodTypes.Year => dateTimeStart.AddDays(-NumberOfDaysPerYear + 1),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Period not implemented")
        };
    }
}