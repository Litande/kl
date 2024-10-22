using KL.Statistics.Application.Common.Enums;
using KL.Statistics.Application.Models;

namespace KL.Statistics.Application.Services;

public interface IStatisticPeriodService
{
    ReportPeriod GetCurrent(PeriodTypes type);
    ReportPeriod GetPrevious(ReportPeriod period);
    IEnumerable<ReportPeriod> SplitToPeriod(ReportPeriod period);
}