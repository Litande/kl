using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;
using Plat4Me.DialLeadCaller.Application.App;
using Plat4Me.DialLeadCaller.Application.Models;
using Plat4Me.DialLeadCaller.Application.Services.Contracts;
using Plat4Me.DialLeadCaller.Infrastructure.App;

namespace Plat4Me.DialLeadCaller.Infrastructure;

public class CallerLoopBackgroundService : BackgroundService
{
    private readonly ILogger<CallerLoopBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly CallerBackgroundOptions _callerBackgroundOptions;
    private readonly IBridgeService _bridgeService;
    private readonly PubSubjects _pubSubjects;
    private readonly INatsPublisher _natsPublisher;
    private readonly TaskCompletionSource _appStarted = new();

    public CallerLoopBackgroundService(
        ILogger<CallerLoopBackgroundService> logger,
        IServiceProvider serviceProvider,
        IOptions<CallerBackgroundOptions> callerBackgroundOptions,
        IBridgeService bridgeService,
        IOptions<PubSubjects> pubSubjects,
        INatsPublisher natsPublisher,
        IHostApplicationLifetime lifetime)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _bridgeService = bridgeService;
        _pubSubjects = pubSubjects.Value;
        _natsPublisher = natsPublisher;
        _callerBackgroundOptions = callerBackgroundOptions.Value;
        lifetime.ApplicationStarted.Register(() => _appStarted.SetResult());
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await _appStarted.Task.ConfigureAwait(false);
        
        await _natsPublisher.PublishAsync(_pubSubjects.BridgeRegRequest, new BridgeRegRequestMessage());

        while (!ct.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("{Service} Starting", nameof(CallerLoopBackgroundService));

                await using var scope = _serviceProvider.CreateAsyncScope();
                var service = scope.ServiceProvider.GetRequiredService<ICallerService>();

                await _bridgeService.PingBridges();
                await service.TryToCallPredictive(ct);

                await Task.Delay(TimeSpan.FromSeconds(_callerBackgroundOptions.TryToCallPeriodSeconds), ct);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during {Service} executing", nameof(CallerLoopBackgroundService));
            }
        }
    }
}
