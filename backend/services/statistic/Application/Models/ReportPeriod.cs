using Plat4Me.Dial.Statistic.Api.Application.Common.Enums;

namespace Plat4Me.Dial.Statistic.Api.Application.Models;

public record ReportPeriod(PeriodTypes Type, DateTimeOffset From, DateTimeOffset To);