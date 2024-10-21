using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;
using Plat4Me.DialClientApi.Application.Configurations;
using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Application.Models.Messages.Call;

namespace Plat4Me.DialClientApi.Application.Handlers;

public class CallFinishByManagerHandler : ICallFinishByManagerHandler
{
    private readonly INatsPublisher _natsPublisher;
    private readonly NatsPubSubOptions _pubSubjects;

    public CallFinishByManagerHandler(INatsPublisher natsPublisher, IOptions<NatsPubSubOptions> options)
    {
        _natsPublisher = natsPublisher;
        _pubSubjects = options.Value;
    }

    public async Task Handle(string sessionId, CancellationToken ct = default)
    {
        var message = new HangupCallMessage(sessionId, CallFinishReasons.CallFinishedByManager);
        await _natsPublisher.PublishAsync(_pubSubjects.HangupCall, message);
    }
}
