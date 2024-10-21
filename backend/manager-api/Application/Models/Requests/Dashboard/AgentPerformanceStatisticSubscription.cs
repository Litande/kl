using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Application.Models.Requests.Dashboard;

public record AgentPerformanceStatisticSubscription(DateTimeOffset From, PerformanceTypes[] Types);