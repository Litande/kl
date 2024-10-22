namespace KL.Statistics.Application.Handlers.Dashboard;

public interface ISenderDashboardPerformanceHandler
{
    Task Handle(CancellationToken ct = default);
}