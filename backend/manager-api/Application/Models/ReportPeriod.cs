using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Application.Models;

public record ReportPeriod(PeriodTypes Type, DateTimeOffset From, DateTimeOffset To);