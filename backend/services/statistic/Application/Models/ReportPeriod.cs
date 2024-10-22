using KL.Statistics.Application.Common.Enums;

namespace KL.Statistics.Application.Models;

public record ReportPeriod(PeriodTypes Type, DateTimeOffset From, DateTimeOffset To);