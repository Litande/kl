using Plat4Me.Dial.Statistic.Api.Application.Common.Enums;

namespace Plat4Me.Dial.Statistic.Api.Application.Models.Requests;

public record AgentPerformanceStatisticSubscription(DateTimeOffset From, PerformanceTypes[] Types);