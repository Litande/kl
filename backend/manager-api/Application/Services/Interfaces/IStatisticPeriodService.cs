using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Application.Models;

namespace Plat4Me.DialClientApi.Application.Services.Interfaces;

public interface IStatisticPeriodService
{
    ReportPeriod GetCurrent(PeriodTypes type);
    ReportPeriod GetPrevious(ReportPeriod period);
    IEnumerable<ReportPeriod> SplitToPeriod(ReportPeriod period);
}