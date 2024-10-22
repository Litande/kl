using System.Collections.Concurrent;
using KL.Caller.Leads.App;
using KL.Caller.Leads.Models;
using KL.Caller.Leads.Services.Contracts;
using KL.Nats;
using Microsoft.Extensions.Options;

namespace KL.Caller.Leads;

public class CallPublishBackgroundService : BackgroundService, ICallPublishService
{
    private readonly ILogger<CallPublishBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptionsMonitor<CallPublishBackgroundOptions> _callPublishBackgroundOptions;
    private readonly PubSubjects _pubSubjects;
    private readonly INatsPublisher _natsPublisher;
    private readonly ConcurrentQueue<CallToLeadMessage> _messages = new();
    private readonly ICallInfoService _callInfoService;

    public CallPublishBackgroundService(
        ILogger<CallPublishBackgroundService> logger,
        IServiceProvider serviceProvider,
        IOptionsMonitor<CallPublishBackgroundOptions> callPublishBackgroundOptions,
        IOptions<PubSubjects> pubSubjects,
        INatsPublisher natsPublisher,
        ICallInfoService callInfoService
        )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _pubSubjects = pubSubjects.Value;
        _natsPublisher = natsPublisher;
        _callPublishBackgroundOptions = callPublishBackgroundOptions;
        _callInfoService = callInfoService;
    }

    public Task Process(CallToLeadMessage message, CancellationToken ct = default)
    {
        _messages.Enqueue(message);
        return Task.CompletedTask;
    }


    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation("{Service} Starting", nameof(CallPublishBackgroundService));
        int counter = 0;
        while (!ct.IsCancellationRequested)
        {
            try
            {
                if (!_messages.TryDequeue(out var message))
                {
                    counter = 0;
                    await Task.Delay(TimeSpan.FromMilliseconds(_callPublishBackgroundOptions.CurrentValue.WaitingPeriod));
                    continue;
                }
                await _natsPublisher.PublishAsync(_pubSubjects.TryCallToLead, message);
                var callInitiatedMsg = new CallInitiatedMessage(
                    message.ClientId,
                    message.BridgeId,
                    message.SessionId,
                    message.CallType,
                    message.AgentId,
                    message.QueueId,
                    message.LeadId,
                    message.Phone);
                await _natsPublisher.PublishAsync(_pubSubjects.CallInitiated, callInitiatedMsg);
                await _callInfoService.AddCallInfo(callInitiatedMsg, ct);

                ++counter;
                if (counter >= _callPublishBackgroundOptions.CurrentValue.MessagesPerIteration)
                {
                    counter = 0;
                    await Task.Delay(TimeSpan.FromMilliseconds(_callPublishBackgroundOptions.CurrentValue.IterationDelay), ct);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during {Service} executing", nameof(CallPublishBackgroundService));
            }
        }
    }
}
