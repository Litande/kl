using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;
using Plat4Me.DialAgentApi.Application.Configurations;
using Plat4Me.DialAgentApi.Application.Models.Messages;
using System.Text.Json;
using Plat4Me.DialAgentApi.Application.Handlers;

namespace Plat4Me.DialAgentApi.Workers;

public class SubscribeHandlersBackgroundService : BackgroundService
{
    private readonly INatsSubscriber _subscriber;
    private readonly NatsPubSubOptions _natsPubSubOptions;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SubscribeHandlersBackgroundService> _logger;

    public SubscribeHandlersBackgroundService(
        INatsSubscriber subscriber,
        IOptions<NatsPubSubOptions> natsPubSubOptions,
        IServiceProvider serviceProvider,
        ILogger<SubscribeHandlersBackgroundService> logger)
    {
        _subscriber = subscriber;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _natsPubSubOptions = natsPubSubOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (!_natsPubSubOptions.Enabled) return;

            _logger.LogInformation("{Service} Starting", nameof(SubscribeHandlersBackgroundService));

            await _subscriber.SubscribeAsync<AgentBlockedMessage>(_natsPubSubOptions.AgentBlocked, SubHandler<IAgentBlockedHandler, AgentBlockedMessage>);
            await _subscriber.SubscribeAsync<InviteAgentMessage>(_natsPubSubOptions.InviteAgent, SubHandler<ICallMessagesHandler, InviteAgentMessage>);
            await _subscriber.SubscribeAsync<AgentReplacedMessage>(_natsPubSubOptions.AgentReplaceResult, SubHandler<ICallMessagesHandler, AgentReplacedMessage>);
            await _subscriber.SubscribeAsync<CallFinishedMessage>(_natsPubSubOptions.CallFinished, SubHandler<ICallMessagesHandler, CallFinishedMessage>);
            await _subscriber.SubscribeAsync<CallFinishedMessage>(_natsPubSubOptions.CallFailed, SubHandler<ICallMessagesHandler, CallFinishedMessage>);
            await _subscriber.SubscribeAsync<CalleeAnsweredMessage>(_natsPubSubOptions.LeadAnswered, SubHandler<ICallMessagesHandler, CalleeAnsweredMessage>);
            await _subscriber.SubscribeAsync<CalleeAnsweredMessage>(_natsPubSubOptions.AgentAnswered, SubHandler<ICallMessagesHandler, CalleeAnsweredMessage>);
            await _subscriber.SubscribeAsync<CallInitiatedMessage>(_natsPubSubOptions.CallInitiated, SubHandler<ICallMessagesHandler, CallInitiatedMessage>);
            await _subscriber.SubscribeAsync<DroppedAgentMessage>(_natsPubSubOptions.DroppedAgent, SubHandler<ICallMessagesHandler, DroppedAgentMessage>);
            await _subscriber.SubscribeAsync<LeadFeedbackFilledMessage>(_natsPubSubOptions.LeadFeedbackFilled, SubHandler<ICallMessagesHandler, LeadFeedbackFilledMessage>);
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
