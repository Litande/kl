using KL.Manager.API.Application.Configurations;
using KL.Manager.API.Application.Enums;
using KL.Manager.API.Application.Models.Messages.Call;
using KL.Nats;
using Microsoft.Extensions.Options;

namespace KL.Manager.API.Application.Handlers;

public class CallFinishByManagerHandler(INatsPublisher natsPublisher, IOptions<NatsPubSubOptions> options)
    : ICallFinishByManagerHandler
{
    private readonly NatsPubSubOptions _pubSubjects = options.Value;

    public async Task Handle(string sessionId, CancellationToken ct = default)
    {
        var message = new HangupCallMessage(sessionId, CallFinishReasons.CallFinishedByManager);
        await natsPublisher.PublishAsync(_pubSubjects.HangupCall, message);
    }
}
