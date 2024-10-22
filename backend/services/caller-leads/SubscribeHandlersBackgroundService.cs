using System.Text.Json;
using KL.Caller.Leads.App;
using KL.Caller.Leads.Handlers.Contracts;
using KL.Caller.Leads.Models.Messages;
using KL.Nats;
using Microsoft.Extensions.Options;

namespace KL.Caller.Leads;

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
        try
        {
            _logger.LogInformation("{Service} Starting", nameof(SubscribeHandlersBackgroundService));

            await _natsSubscriber.SubscribeAsync<BridgeRunMessage>(_subSubjects.BridgeRun, SubHandler<IBridgeRunHandler, BridgeRunMessage>);
            await _natsSubscriber.SubscribeAsync<CalleeAnsweredMessage>(_subSubjects.AgentAnswered, SubHandler<IAgentAnsweredHandler, CalleeAnsweredMessage>);
            await _natsSubscriber.SubscribeAsync<CalleeAnsweredMessage>(_subSubjects.LeadAnswered, SubHandler<ILeadAnsweredHandler, CalleeAnsweredMessage>);
            await _natsSubscriber.SubscribeAsync<CallFinishedMessage>(_subSubjects.CallFinished, SubHandler<ICallFinishedHandler, CallFinishedMessage>);
            await _natsSubscriber.SubscribeAsync<CallFinishedMessage>(_subSubjects.CallFailed, SubHandler<ICallFailedHandler, CallFinishedMessage>);
            await _natsSubscriber.SubscribeAsync<CallFinishedRecordsMessage>(_subSubjects.CallFinishedRecords, SubHandler<ICallFinishedRecordsHandler, CallFinishedRecordsMessage>);
            await _natsSubscriber.SubscribeAsync<AgentNotAnsweredMessage>(_subSubjects.AgentNotAnswered, SubHandler<IAgentNotAnsweredHandler, AgentNotAnsweredMessage>);
            await _natsSubscriber.SubscribeAsync<LeadFeedbackFilledMessage>(_subSubjects.LeadFeedbackFilled, SubHandler<ILeadFeedbackFilledHandler, LeadFeedbackFilledMessage>);
            await _natsSubscriber.SubscribeAsync<MixedRecordReadyMessage>(_subSubjects.MixedRecordReady, SubHandler<IMixedRecordReadyHandler, MixedRecordReadyMessage>);
            await _natsSubscriber.SubscribeAsync<ManualCallMessage>(_subSubjects.ManualCall, SubHandler<IManualCallHandler, ManualCallMessage>);
            await _natsSubscriber.SubscribeAsync<CallAgainMessage>(_subSubjects.CallAgain, SubHandler<ICallAgainHandler, CallAgainMessage>);
            await _natsSubscriber.SubscribeAsync<DroppedAgentMessage>(_subSubjects.DroppedAgent, SubHandler<IDroppedAgentHandler, DroppedAgentMessage>);
            await _natsSubscriber.SubscribeAsync<EnqueueAgentForCallMessage>(_subSubjects.EnqueueAgentForCall, SubHandler<IEnqueueAgentForCallHandler, EnqueueAgentForCallMessage>);
            await _natsSubscriber.SubscribeAsync<DequeueAgentForCallMessage>(_subSubjects.DequeueAgentForCall, SubHandler<IDequeueAgentForCallHandler, DequeueAgentForCallMessage>);
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
