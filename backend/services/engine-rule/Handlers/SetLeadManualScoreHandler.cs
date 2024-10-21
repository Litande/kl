using Microsoft.Extensions.Logging;
using Plat4Me.DialRuleEngine.Application.Repositories;
using Plat4Me.DialRuleEngine.Application.Handlers.Contracts;
using Plat4Me.DialRuleEngine.Application.Services.Contracts;

namespace Plat4Me.DialRuleEngine.Application.Handlers;

public class SetLeadManualScoreHandler : ISetLeadManualScoreHandler
{
    private readonly ILogger<SetLeadManualScoreHandler> _logger;
    private readonly ILeadsQueueStore _leadsQueueStore;
    private readonly ILeadsQueueUpdateNotificationHandler _leadsQueueUpdateNotificationHandler;
    private readonly ILeadLastCacheRepository _leadLastCacheRepository;
    private readonly IQueueLeadsCacheRepository _queueLeadsCacheRepository;

    public SetLeadManualScoreHandler(
        ILogger<SetLeadManualScoreHandler> logger,
        ILeadsQueueStore leadsQueueStore,
        ILeadsQueueUpdateNotificationHandler leadsQueueUpdateNotificationHandler,
        ILeadLastCacheRepository leadLastCacheRepository,
        IQueueLeadsCacheRepository queueLeadsCacheRepository)
    {
        _logger = logger;
        _leadsQueueStore = leadsQueueStore;
        _leadsQueueUpdateNotificationHandler = leadsQueueUpdateNotificationHandler;
        _leadLastCacheRepository = leadLastCacheRepository;
        _queueLeadsCacheRepository = queueLeadsCacheRepository;
    }

    public async Task Process(long clientId, long queueId, long leadId, long score, CancellationToken ct = default)
    {
        _logger.LogInformation("Set manual score {score} for lead Id {leadId} in queue Id {queueId} for client Id {clientId}",
            score, leadId, queueId, clientId);

        _leadsQueueStore.SetLeadManualScore(clientId, leadId, score);

        await _leadLastCacheRepository.UpdateScore(leadId, score);
        await _queueLeadsCacheRepository.UpdateScore(clientId, queueId, leadId, score);
        await _leadsQueueUpdateNotificationHandler.Process(clientId, queueId);
    }
}
