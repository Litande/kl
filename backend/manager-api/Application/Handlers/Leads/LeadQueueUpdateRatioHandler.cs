using KL.Manager.API.Application.Configurations;
using KL.Manager.API.Application.Models.Messages.LeadQueue;
using KL.Manager.API.Persistent.Entities;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;

namespace KL.Manager.API.Application.Handlers.Leads;

public class LeadQueueUpdateRatioHandler : ILeadQueueUpdateRatioHandler
{
    private readonly ILeadQueueRepository _leadQueueRepository;
    private readonly INatsPublisher _natsPublisher;
    private readonly NatsPubSubOptions _pubSubjects;

    public LeadQueueUpdateRatioHandler(
        ILeadQueueRepository leadQueueRepository,
        INatsPublisher natsPublisher,
        IOptions<NatsPubSubOptions> natsOptions)
    {
        _leadQueueRepository = leadQueueRepository;
        _natsPublisher = natsPublisher;
        _pubSubjects = natsOptions.Value;
    }

    public async Task<LeadQueue?> Handle(long clientId, long leadQueueId, double ratio, CancellationToken ct = default)
    {
        var leadQueue = await _leadQueueRepository.UpdateRatio(clientId, leadQueueId, ratio, ct);

        if (leadQueue is null)
            return null;

        var message = new RatioChangedMessage(leadQueue.ClientId, leadQueue.Id, leadQueue.Ratio);
        await _natsPublisher.PublishAsync(_pubSubjects.RatioChanged, message);

        return leadQueue;
    }
}
