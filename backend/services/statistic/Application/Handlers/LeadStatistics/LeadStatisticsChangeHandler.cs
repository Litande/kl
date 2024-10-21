using Plat4Me.Dial.Statistic.Api.Application.Models.Messages;
using Plat4Me.Dial.Statistic.Api.Application.SignalR;
using Plat4Me.Dial.Statistic.Api.DAL.Repositories;

namespace Plat4Me.Dial.Statistic.Api.Application.Handlers.LeadStatistics;

public class LeadStatisticsChangeHandler : ILeadStatisticsChangeHandler
{
    private readonly IHubSender _hubSender;
    private readonly ILogger<LeadStatisticsChangeHandler> _logger;
    private readonly ILeadStatisticCacheRepository _leadStatisticCache;

    public LeadStatisticsChangeHandler(
        ILogger<LeadStatisticsChangeHandler> logger,
        ILeadStatisticCacheRepository leadStatisticCache,
        IHubSender hubSender)
    {
        _logger = logger;
        _leadStatisticCache = leadStatisticCache;
        _hubSender = hubSender;
    }

    public async Task Process(LeadsStatisticUpdateMessage message, CancellationToken ct = default)
    {
        var leadStatistics = await _leadStatisticCache.GetLeadStatisticByClient(message.ClientId);

        if (leadStatistics is null)
            return;

        await _hubSender.SendLeadStatistic(message.ClientId, leadStatistics.Statistics, ct);

        _logger.LogInformation("Sent new leads statistics: {0}", nameof(leadStatistics));
    }
}
