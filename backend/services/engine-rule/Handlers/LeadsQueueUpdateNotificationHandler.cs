using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;
using Plat4Me.DialRuleEngine.Application.App;
using Plat4Me.DialRuleEngine.Application.Handlers.Contracts;
using Plat4Me.DialRuleEngine.Application.Models;

namespace Plat4Me.DialRuleEngine.Application.Handlers;

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