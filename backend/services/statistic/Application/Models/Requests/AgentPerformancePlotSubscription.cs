using KL.Statistics.Application.Common.Enums;

namespace KL.Statistics.Application.Models.Requests;

public record AgentPerformancePlotSubscription(DateTimeOffset From, int Step, PerformanceTypes Type);