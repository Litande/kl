using System.Text.Json;
using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;
using Plat4Me.DialClientApi.Application.Configurations;
using Plat4Me.DialClientApi.Application.Handlers;
using Plat4Me.DialClientApi.Application.Handlers.LeadGroups;
using Plat4Me.DialClientApi.Application.Handlers.LiveTracking;
using Plat4Me.DialClientApi.Application.Models.Messages.Agents;
using Plat4Me.DialClientApi.Application.Models.Messages.LeadGroups;
using Plat4Me.DialClientApi.Application.Models.Messages.RuleEngine;

namespace Plat4Me.DialClientApi.Workers;

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
        _natsPubSubOptions = natsPubSubOptions.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (!_natsPubSubOptions.Enabled) return;

            _logger.LogInformation("{Service} Starting", nameof(SubscribeHandlersBackgroundService));

            await _subscriber.SubscribeAsync<AgentChangedStatusMessage>(_natsPubSubOptions.AgentChangedStatus, SubHandler<IAgentChangedStatusHandler, AgentChangedStatusMessage>);
            await _subscriber.SubscribeAsync<QueuesUpdatedMessage>(_natsPubSubOptions.LeadsQueueUpdate, SubHandler<IQueuesUpdatedHandler, QueuesUpdatedMessage>);
            await _subscriber.SubscribeAsync<RuleEngineRunMessage>(_natsPubSubOptions.RuleEngineRun, SubHandler<IRuleEngineRunHandler, RuleEngineRunMessage>);
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
