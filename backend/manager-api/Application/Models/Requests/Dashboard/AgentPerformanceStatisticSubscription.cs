using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Models.Requests.Dashboard;

public record AgentPerformanceStatisticSubscription(DateTimeOffset From, PerformanceTypes[] Types);