namespace Plat4Me.Dial.Statistic.Api.Application.Handlers.Dashboard;

public interface ICallAnalysisChangedHandler
{
    Task Handle(CancellationToken ct = default);
}