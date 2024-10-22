using KL.Manager.API.Application.Enums;
using KL.Manager.API.Application.Models;

namespace KL.Manager.API.Application.Services.Interfaces;

public interface IStatisticPeriodService
{
    ReportPeriod GetCurrent(PeriodTypes type);
    ReportPeriod GetPrevious(ReportPeriod period);
    IEnumerable<ReportPeriod> SplitToPeriod(ReportPeriod period);
}