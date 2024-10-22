using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Models;

public record ReportPeriod(PeriodTypes Type, DateTimeOffset From, DateTimeOffset To);