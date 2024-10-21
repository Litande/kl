using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Application.Models.Requests.Dashboard;

public record AgentPerformancePlotSubscription(DateTimeOffset From, int Step, PerformanceTypes Type);