using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;
using Plat4Me.Dial.Statistic.Api.Application.Handlers;
using Plat4Me.Dial.Statistic.Api.Application.Handlers.Agent;
using Plat4Me.Dial.Statistic.Api.Application.Handlers.LeadStatistics;
using Plat4Me.Dial.Statistic.Api.Application.Models.Messages;
using Plat4Me.Dial.Statistic.Api.Configurations;
using System.Text.Json;

namespace Plat4Me.Dial.Statistic.Api.Workers;

public class SubscribeHandlersBackgroundService : BackgroundService
{
    private readonly INatsSubscriber _subscriber;
    private readonly NatsPubSubOptions _natsPubSubOptions;
    private readonly SubSubjects _subSubjects;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SubscribeHandlersBackgroundService> _logger;

    public SubscribeHandlersBackgroundService(
        INatsSubscriber subscriber,
        IOptions<NatsPubSubOptions> natsPubSubOptions,
        IOptions<SubSubjects> subSubjects,
        IServiceProvider serviceProvider,
        ILogger<SubscribeHandlersBackgroundService> logger)
    {
        _subscriber = subscriber;
        _natsPubSubOptions = natsPubSubOptions.Value;
        _subSubjects = subSubjects.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (!_natsPubSubOptions.Enabled) return;

            _logger.LogInformation("{Service} Starting", nameof(SubscribeHandlersBackgroundService));

            await using var scope = _serviceProvider.CreateAsyncScope();
            var service = scope.ServiceProvider.GetRequiredService<ICdrUpdatedService>();

            await _subscriber.SubscribeAsync<AgentsChangedStatusMessage>(_natsPubSubOptions.AgentChangedStatus, SubHandler<IAgentStatisticsChangeHandler, AgentsChangedStatusMessage>);
            await _subscriber.SubscribeAsync<LeadsStatisticUpdateMessage>(_natsPubSubOptions.LeadsStatisticsUpdate, SubHandler<ILeadStatisticsChangeHandler, LeadsStatisticUpdateMessage>);

            await _subscriber.SubscribeAsync<CdrUpdatedMessage>(_subSubjects.CdrUpdated, CdrUpdatedService);
            void CdrUpdatedService(CdrUpdatedMessage message) => service.AddToQueueUpdatedCdrProcess(message, stoppingToken);

            await _subscriber.SubscribeAsync<CdrUpdatedMessage>(_subSubjects.CdrInserted, CdrInsertedService);
            void CdrInsertedService(CdrUpdatedMessage message) => service.AddToQueueInsertedCdrProcess(message, stoppingToken);
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