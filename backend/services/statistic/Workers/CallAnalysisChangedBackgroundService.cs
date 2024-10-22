using KL.Statistics.Application.Handlers.Dashboard;
using KL.Statistics.Configurations;
using Microsoft.Extensions.Options;

namespace KL.Statistics.Workers;

public class CallAnalysisChangedBackgroundService : BackgroundService
{
    private readonly ILogger<CallAnalysisChangedBackgroundService> _logger;
    private readonly DashboardPerformanceOptions _options;
    private readonly IServiceProvider _serviceProvider;
    public CallAnalysisChangedBackgroundService(
        ILogger<CallAnalysisChangedBackgroundService> logger,
        IOptions<DashboardPerformanceOptions> options,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _options = options.Value;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("{Service} Starting", nameof(CallAnalysisChangedBackgroundService));

            while (!stoppingToken.IsCancellationRequested)
            {
                await using var scope = _serviceProvider.CreateAsyncScope();
                var service = scope.ServiceProvider.GetRequiredService<ICallAnalysisChangedHandler>();

                await service.Handle(stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(_options.RunCallAnalysisIntervalSeconds), stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during {Service} executing", nameof(CallAnalysisChangedBackgroundService));
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}