using System.Text.Json;
using KL.Nats;
using KL.SIP.Bridge.Application.Configurations;
using KL.SIP.Bridge.Application.Handlers;
using KL.SIP.Bridge.Application.Models.Messages;
using Microsoft.Extensions.Options;

namespace KL.SIP.Bridge.Application.Workers;

public class SubscribeHandlersBackgroundService : BackgroundService
{
    private readonly INatsSubscriber _subscriber;
    private readonly NatsSubjects _natsSubOptions;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SubscribeHandlersBackgroundService> _logger;
    private readonly TaskCompletionSource _appStarted = new();

    public SubscribeHandlersBackgroundService(
        INatsSubscriber subscriber,
        IOptions<NatsSubjects> natsPubSubOptions,
        IServiceProvider serviceProvider,
        ILogger<SubscribeHandlersBackgroundService> logger,
        IHostApplicationLifetime lifetime
        )
    {
        _subscriber = subscriber;
        _natsSubOptions = natsPubSubOptions.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;
        lifetime.ApplicationStarted.Register(() => _appStarted.SetResult());
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _appStarted.Task.ConfigureAwait(false);
        try
        {
            _logger.LogInformation("{Service} Starting", nameof(SubscribeHandlersBackgroundService));

            SubHandler<IBridgeRegRequestHandler, BridgeRegRequestMessage>(new BridgeRegRequestMessage(Initiator: nameof(KL.SIP.Bridge)));

            await _subscriber.SubscribeAsync<BridgeRegRequestMessage>(_natsSubOptions.BridgeRegRequest, SubHandler<IBridgeRegRequestHandler, BridgeRegRequestMessage>);
            await _subscriber.SubscribeAsync<CallToLeadMessage>(_natsSubOptions.TryCallToLead, SubHandler<ICallToLeadHandler, CallToLeadMessage>);
            await _subscriber.SubscribeAsync<AgentReplacedMessage>(_natsSubOptions.AgentReplaceResult, SubHandler<IAgentReplaceDataHandler, AgentReplacedMessage>);
            await _subscriber.SubscribeAsync<HangupCallMessage>(_natsSubOptions.HangupCall, SubHandler<IHangupCallHandler, HangupCallMessage>);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during {Service} executing", nameof(SubscribeHandlersBackgroundService));
            throw;
        }
    }

    private async void SubHandler<THandler, TMessage>(TMessage message)
        where THandler : ISubHandler<TMessage>
    {
        try
        {
            _logger.LogInformation("{SubHandler} Starting with message {message}",
                typeof(THandler), JsonSerializer.Serialize(message));

            await using var scope = _serviceProvider.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<THandler>();
            await handler.Process(message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during {SubHandler} executing", typeof(THandler));
        }
    }
}
