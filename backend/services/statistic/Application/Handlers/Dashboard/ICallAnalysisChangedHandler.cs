namespace KL.Statistics.Application.Handlers.Dashboard;

public interface ICallAnalysisChangedHandler
{
    Task Handle(CancellationToken ct = default);
}