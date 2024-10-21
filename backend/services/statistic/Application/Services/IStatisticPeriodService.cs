using Plat4Me.Dial.Statistic.Api.Application.Common.Enums;
using Plat4Me.Dial.Statistic.Api.Application.Models;

namespace Plat4Me.Dial.Statistic.Api.Application.Services;

public interface IStatisticPeriodService
{
    ReportPeriod GetCurrent(PeriodTypes type);
    ReportPeriod GetPrevious(ReportPeriod period);
    IEnumerable<ReportPeriod> SplitToPeriod(ReportPeriod period);
}