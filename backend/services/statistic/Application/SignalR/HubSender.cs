using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Plat4Me.Dial.Statistic.Api.Application.Models;
using Plat4Me.Dial.Statistic.Api.Application.Models.Responses;
using Plat4Me.Dial.Statistic.Api.Application.Models.StatisticCache;
using System.Collections.Concurrent;

namespace Plat4Me.Dial.Statistic.Api.Application.SignalR;

public class HubSender : IHubSender
{
    public ConcurrentDictionary<string, PerformanceSubscriber> PerformanceSubscribers =>
        TrackingHub.PerformanceSubscribers;

    private readonly IHubContext<TrackingHub> _trackingHub;
    private readonly SignalROptions _options;

    public HubSender(
        IHubContext<TrackingHub> trackingHub,
        IOptions<SignalROptions> options)
    {
        _trackingHub = trackingHub;
        _options = options.Value;
    }

    public async Task SendCallAnalysis(
        long clientId,
        IEnumerable<CallAnalysisResponse> callAnalysis,
        CancellationToken ct = default)
    {
        await _trackingHub.Clients
            .Group(TrackingHub.GetGroupName(clientId))
            .SendAsync(_options.CallAnalysis, callAnalysis, cancellationToken: ct);
    }

    public async Task SendLeadGroups(
        long clientId,
        IEnumerable<LeadGroup> leadGroups,
        CancellationToken ct = default)
    {
        await _trackingHub.Clients
            .Group(TrackingHub.GetGroupName(clientId))
            .SendAsync(_options.LeadGroups, leadGroups, cancellationToken: ct);
    }

    public async Task SendAgentWorkMode(
        long clientId,
        AgentsWorkMode agentsWorkMode,
        CancellationToken ct = default)
    {
        await _trackingHub.Clients
            .Group(TrackingHub.GetGroupName(clientId))
            .SendAsync(_options.AgentsWorkMode, agentsWorkMode, cancellationToken: ct);
    }

    public async Task SendLeadStatistic(
        long clientId,
        IEnumerable<LeadStatisticCache> statistics,
        CancellationToken ct = default)
    {
        await _trackingHub.Clients
            .Group(TrackingHub.GetGroupName(clientId))
            .SendAsync(_options.LeadStatistics, statistics, cancellationToken: ct);
    }

    public async Task SendPerformanceStatistic(
        long clientId,
        string connectionId,
        IEnumerable<PerformanceStatisticsData> statistics,
        CancellationToken ct = default)
    {
        await _trackingHub.Clients
            .Client(connectionId)
            .SendAsync(_options.PerformanceStatistic, statistics, cancellationToken: ct);
    }

    public async Task SendPerformancePlot(
        long clientId,
        string connectionId,
        PerformancePlotDataItem? value,
        CancellationToken ct = default)
    {
        await _trackingHub.Clients
            .Client(connectionId)
            .SendAsync(_options.PerformancePlot, value, cancellationToken: ct);
    }
}