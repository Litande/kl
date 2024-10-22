using System.Text.Json;
using KL.Provider.Leads.Application.Configurations;
using KL.Provider.Leads.Application.Handlers.Interfaces;
using KL.Provider.Leads.Application.Models.Messages;
using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;

namespace KL.Provider.Leads.Workers;

public class NatsListenerBackgroundService : BackgroundService
{
    private readonly ILogger<NatsListenerBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly SubSubjects _subSubjects;
    private readonly INatsSubscriber _natsSubscriber;

    public NatsListenerBackgroundService(
        ILogger<NatsListenerBackgroundService> logger,
        IServiceProvider serviceProvider,
        IOptions<SubSubjects> subSubjects,
        INatsSubscriber natsSubscriber)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _natsSubscriber = natsSubscriber;
        _subSubjects = subSubjects.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await SubscribeHandlers(ct);
    }

    private async Task SubscribeHandlers(CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("{Service} Starting", nameof(SubscribeHandlers));

            await _natsSubscriber.SubscribeAsync<LeadFeedbackProcessedMessage>(_subSubjects.LeadFeedbackProcessed,
                SubHandler<ILeadFeedbackProcessedHandler, LeadFeedbackProcessedMessage>);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during {Service} executing", nameof(SubscribeHandlers));
            throw;
        }
    }

    private async void SubHandler<THandler, TMessage>(TMessage message)
        where THandler : ISubHandler<TMessage>
    {
        try
        {
            _logger.LogInformation("{Handler} Starting with message {Message}",
                typeof(THandler), JsonSerializer.Serialize(message));

            await using var scope = _serviceProvider.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<THandler>();
            await handler.Process(message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during {Handler} executing", typeof(THandler));
        }
    }
}