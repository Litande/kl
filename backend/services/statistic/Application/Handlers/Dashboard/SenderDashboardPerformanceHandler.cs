using Plat4Me.Dial.Statistic.Api.Application.Models;
using Plat4Me.Dial.Statistic.Api.Application.SignalR;
using System.Collections.Concurrent;

namespace Plat4Me.Dial.Statistic.Api.Application.Handlers.Dashboard;

public class SenderDashboardPerformanceHandler : ISenderDashboardPerformanceHandler
{
    private readonly ConcurrentDictionary<string, PerformanceSubscriber> _subscribers =
        TrackingHub.PerformanceSubscribers;

    private readonly IPerformanceStatisticQueryHandler _performanceStatisticQueryHandler;
    private readonly IPerformancePlotQueryHandler _performancePlotQueryHandler;
    private readonly ILogger<SenderDashboardPerformanceHandler> _logger;
    private readonly IHubSender _hubSender;

    public SenderDashboardPerformanceHandler(
        IPerformanceStatisticQueryHandler performanceStatisticQueryHandler,
        IPerformancePlotQueryHandler performancePlotQueryHandler,
        ILogger<SenderDashboardPerformanceHandler> logger,
        IHubSender hubSender)
    {
        _performanceStatisticQueryHandler = performanceStatisticQueryHandler;
        _performancePlotQueryHandler = performancePlotQueryHandler;
        _logger = logger;
        _hubSender = hubSender;
    }

    public async Task Handle(CancellationToken ct = default)
    {
        foreach (var subscriber in _subscribers)
        {
            var clientId = subscriber.Value.ClientId;
            var plot = subscriber.Value.PlotSubscription;
            var statistic = subscriber.Value.StatisticSubscription;

            if (plot is not null)
                await SendPerformancePlot(clientId, subscriber.Key, plot, ct);
            if (statistic is not null)
                await SendPerformanceStatistic(clientId, subscriber.Key, statistic, ct);
        }
    }

    private async Task SendPerformanceStatistic(
        long clientId,
        string subscriberKey,
        StatisticSubscription subscription,
        CancellationToken ct)
    {
        try
        {
            var data = await _performanceStatisticQueryHandler
                .Handle(clientId, subscription.Types, subscription.From,
                    DateTimeOffset.UtcNow, ct);

            await _hubSender.SendPerformanceStatistic(clientId, subscriberKey, data, ct);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during {Service} executing with subscription {Subscription}",
                nameof(SendPerformanceStatistic), subscription.ToString());
        }
    }

    private async Task SendPerformancePlot(
        long clientId,
        string connectionId,
        PlotSubscription subscription,
        CancellationToken ct)
    {
        try
        {
            var data = await _performancePlotQueryHandler
                .Handle(clientId, subscription.Type, subscription.From,
                    DateTimeOffset.UtcNow, subscription.Step, ct);

            await _hubSender.SendPerformancePlot(clientId, connectionId, data.Values.LastOrDefault(), ct);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during {Service} executing with subscription {Subscription}",
                nameof(SendPerformancePlot), subscription.ToString());
        }
    }
}