namespace Plat4Me.Dial.Statistic.Api.Application.Handlers.Dashboard;

public interface ISenderDashboardPerformanceHandler
{
    Task Handle(CancellationToken ct = default);
}