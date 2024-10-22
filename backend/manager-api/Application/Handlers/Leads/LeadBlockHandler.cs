using KL.Manager.API.Application.Configurations;
using KL.Manager.API.Application.Models.Messages.Lead;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using KL.Nats;
using Microsoft.Extensions.Options;

namespace KL.Manager.API.Application.Handlers.Leads;

public class LeadBlockHandler : IBlockLeadHandler
{
    private readonly ILeadBlacklistRepository _leadBlacklistRepository;
    private readonly INatsPublisher _natsPublisher;
    private readonly NatsPubSubOptions _pubSubjects;
    private readonly IQueueLeadsCacheRepository _queueLeadsCacheRepository;

    public LeadBlockHandler(
        ILeadBlacklistRepository leadBlacklistRepository,
        INatsPublisher natsPublisher,
        IOptions<NatsPubSubOptions> options,
        IQueueLeadsCacheRepository queueLeadsCacheRepository)
    {
        _leadBlacklistRepository = leadBlacklistRepository;
        _natsPublisher = natsPublisher;
        _queueLeadsCacheRepository = queueLeadsCacheRepository;
        _pubSubjects = options.Value;
    }

    public async Task Handle(long clientId, long userId, long leadId, CancellationToken ct = default)
    {
        await _leadBlacklistRepository.Create(clientId, userId, leadId, ct);
        var queuedLead = await _queueLeadsCacheRepository.Get(leadId);
        var message = new LeadBlockedMessage(clientId, leadId, queuedLead?.QueueId);
        await _natsPublisher.PublishAsync(_pubSubjects.LeadBlocked, message);
    }
}