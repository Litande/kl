using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;
using Plat4Me.DialClientApi.Application.Configurations;
using Plat4Me.DialClientApi.Application.Models.Messages.LeadQueue;
using Plat4Me.DialClientApi.Persistent.Entities;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialClientApi.Application.Handlers.Leads;

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
