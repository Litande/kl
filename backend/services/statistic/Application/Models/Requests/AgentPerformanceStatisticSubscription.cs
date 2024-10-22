using KL.Statistics.Application.Common.Enums;

namespace KL.Statistics.Application.Models.Requests;

public record AgentPerformanceStatisticSubscription(DateTimeOffset From, PerformanceTypes[] Types);