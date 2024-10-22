using KL.Engine.Rule.Handlers.Contracts;
using KL.Engine.Rule.Models.Messages;
using KL.Engine.Rule.Repositories;
using KL.Engine.Rule.Services.Contracts;

namespace KL.Engine.Rule.Handlers;

public class LeadBlockedHandler : ILeadBlockedHandler
{
    private readonly IQueueLeadsCacheRepository _queueLeadsCacheRepository;
    private readonly ILeadsQueueStore _leadsQueueStore;
    private readonly ILeadsQueueUpdateNotificationHandler _updateNotificationHandler;
    private readonly ILogger<LeadBlockedHandler> _logger;

    public LeadBlockedHandler(
        IQueueLeadsCacheRepository queueLeadsCacheRepository,
        ILeadsQueueStore leadsQueueStore,
        ILeadsQueueUpdateNotificationHandler updateNotificationHandler,
        ILogger<LeadBlockedHandler> logger)
    {
        _queueLeadsCacheRepository = queueLeadsCacheRepository;
        _leadsQueueStore = leadsQueueStore;
        _updateNotificationHandler = updateNotificationHandler;
        _logger = logger;
    }

    public async Task Process(LeadBlockedMessage message)
    {
        await _queueLeadsCacheRepository.Remove(message.ClientId, message.QueueId, message.LeadId);
        var isSuccess = _leadsQueueStore.TryRemoveLead(message.ClientId, message.LeadId);

        if (isSuccess)
        {
            _logger.LogInformation("Lead {LeadId} has been removed from queue {LeadQueueId} client {ClientId}",
                message.LeadId, message.QueueId, message.QueueId);

            await _updateNotificationHandler.Process(message.ClientId, message.QueueId);
        }
    }
}