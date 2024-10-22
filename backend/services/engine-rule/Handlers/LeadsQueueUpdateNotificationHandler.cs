using KL.Engine.Rule.App;
using KL.Engine.Rule.Handlers.Contracts;
using KL.Engine.Rule.Models;
using KL.Nats;
using Microsoft.Extensions.Options;

namespace KL.Engine.Rule.Handlers;

public class LeadsQueueUpdateNotificationHandler : ILeadsQueueUpdateNotificationHandler
{
    private readonly INatsPublisher _natsPublisher;
    private readonly PubSubjects _pubSubjects;

    public LeadsQueueUpdateNotificationHandler(
        INatsPublisher natsPublisher,
        IOptions<PubSubjects> pubSubjects)
    {
        _natsPublisher = natsPublisher;
        _pubSubjects = pubSubjects.Value;
    }

    public async Task Process(long clientId)
    {
        var message = new QueuesUpdatedMessage(clientId);
        await _natsPublisher.PublishAsync(_pubSubjects.LeadsQueueUpdate, message);
    }

    public async Task Process(long clientId, long queueId)
    {
        var message = new QueuesUpdatedMessage(clientId, new[] { queueId });
        await _natsPublisher.PublishAsync(_pubSubjects.LeadsQueueUpdate, message);
    }

    public async Task Process(long clientId, long? queueId)
    {
        if (queueId.HasValue)
            await Process(clientId, queueId.Value);
        else
            await Process(clientId);
    }
}