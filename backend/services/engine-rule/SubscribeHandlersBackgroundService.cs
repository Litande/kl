using System.Text.Json;
using KL.Engine.Rule.App;
using KL.Engine.Rule.Handlers.Contracts;
using KL.Engine.Rule.Models.Messages;
using KL.Nats;
using Microsoft.Extensions.Options;

namespace KL.Engine.Rule;

public class SubscribeHandlersBackgroundService : BackgroundService
{
    private readonly ILogger<SubscribeHandlersBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly SubSubjects _subSubjects;
    private readonly INatsSubscriber _natsSubscriber;

    public SubscribeHandlersBackgroundService(
        ILogger<SubscribeHandlersBackgroundService> logger,
        IServiceProvider serviceProvider,
        IOptions<SubSubjects> subSubjects,
        INatsSubscriber natsSubscriber)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _subSubjects = subSubjects.Value;
        _natsSubscriber = natsSubscriber;
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

            await _natsSubscriber.SubscribeAsync<LeadFeedbackFilledMessage>(_subSubjects.LeadFeedbackFilled, SubHandler<ILeadFeedbackFilledHandler, LeadFeedbackFilledMessage>);
            await _natsSubscriber.SubscribeAsync<LeadFeedbackFilledMessage>(_subSubjects.LeadFeedbackCallFailed, SubHandler<ILeadFeedbackCallFailedHandler, LeadFeedbackFilledMessage>);
            await _natsSubscriber.SubscribeAsync<LeadsImportedMessage>(_subSubjects.LeadsImported, SubHandler<ILeadsImportedHandler, LeadsImportedMessage>);
            await _natsSubscriber.SubscribeAsync<LeadBlockedMessage>(_subSubjects.LeadBlocked, SubHandler<ILeadBlockedHandler, LeadBlockedMessage>);
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
