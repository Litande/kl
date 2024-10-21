using Microsoft.Extensions.Logging;
using Plat4Me.DialRuleEngine.Application.Handlers.Contracts;
using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.Repositories;
using Plat4Me.DialRuleEngine.Application.Services.Contracts;

namespace Plat4Me.DialRuleEngine.Application.Handlers;

public class LeadQueueStoreUpdateHandler : ILeadQueueStoreUpdateHandler
{
    private readonly ILogger<LeadQueueStoreUpdateHandler> _logger;
    private readonly ILeadsQueueStore _leadsQueueStore;
    private readonly ILeadsQueueUpdateNotificationHandler _updateNotificationHandler;
    private readonly IQueueLeadsCacheRepository _queueLeadsCacheRepository;
    private readonly ILeadRepository _leadRepository;

    public LeadQueueStoreUpdateHandler(
        ILogger<LeadQueueStoreUpdateHandler> logger,
        ILeadsQueueStore leadsQueueStore,
        ILeadsQueueUpdateNotificationHandler updateNotificationHandler,
        IQueueLeadsCacheRepository queueLeadsCacheRepository,
        ILeadRepository leadRepository)
    {
        _logger = logger;
        _leadsQueueStore = leadsQueueStore;
        _updateNotificationHandler = updateNotificationHandler;
        _queueLeadsCacheRepository = queueLeadsCacheRepository;
        _leadRepository = leadRepository;
    }

    public async Task Process(
        long clientId,
        IReadOnlyCollection<TrackedLead> trackedLeads,
        CancellationToken ct = default)
    {
        var leadIds = trackedLeads.Select(x => x.LeadId);

        _logger.LogInformation("Lead queue store updated with lead Ids {leadIds} and queue Ids {queueIds} for client Id {clientId}",
            string.Join(", ", leadIds),
            string.Join(", ", trackedLeads.Select(r => r.LeadQueueId).Distinct()),
            clientId);

        LoggingLeadsWithoutQueueId(trackedLeads);

        _leadsQueueStore.UpdateAll(clientId, trackedLeads);
        await _queueLeadsCacheRepository.UpdateAll(clientId, trackedLeads, ct);
        await _leadRepository.UpdateFirstTimeQueued(leadIds, ct);
        await _updateNotificationHandler.Process(clientId);
    }

    private void LoggingLeadsWithoutQueueId(IEnumerable<TrackedLead> leads)
    {
        var leadsWithoutQueueId = leads
            .Where(r => !r.LeadQueueId.HasValue)
            .Select(r => r.LeadId)
            .ToArray();

        if (leadsWithoutQueueId.Any())
            _logger.LogInformation("The leads with Id {leadIds} do not have any queueId",
                string.Join(", ", leadsWithoutQueueId));
    }
}