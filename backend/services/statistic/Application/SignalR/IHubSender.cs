using Plat4Me.Dial.Statistic.Api.Application.Models;
using Plat4Me.Dial.Statistic.Api.Application.Models.Responses;
using Plat4Me.Dial.Statistic.Api.Application.Models.StatisticCache;
using System.Collections.Concurrent;

namespace Plat4Me.Dial.Statistic.Api.Application.SignalR;

public interface IHubSender
{
    ConcurrentDictionary<string, PerformanceSubscriber> PerformanceSubscribers { get; }
    Task SendLeadGroups(long clientId, IEnumerable<LeadGroup> leadGroups, CancellationToken ct = default);
    Task SendCallAnalysis(long clientId, IEnumerable<CallAnalysisResponse> callAnalysis, CancellationToken ct = default);
    Task SendAgentWorkMode(long clientId, AgentsWorkMode agentsWorkMode, CancellationToken ct = default);
    Task SendLeadStatistic(long clientId, IEnumerable<LeadStatisticCache> statistics, CancellationToken ct = default);
    Task SendPerformanceStatistic(long clientId, string connectionId, IEnumerable<PerformanceStatisticsData> statistics, CancellationToken ct = default);
    Task SendPerformancePlot(long clientId, string connectionId, PerformancePlotDataItem? value, CancellationToken ct = default);
}