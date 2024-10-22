using KL.Statistics.Application.Models.Messages;
using KL.Statistics.Application.SignalR;
using KL.Statistics.DAL.Repositories;

namespace KL.Statistics.Application.Handlers.LeadStatistics;

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
