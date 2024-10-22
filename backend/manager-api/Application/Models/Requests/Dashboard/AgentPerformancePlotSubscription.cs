using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Models.Requests.Dashboard;

public record AgentPerformancePlotSubscription(DateTimeOffset From, int Step, PerformanceTypes Type);