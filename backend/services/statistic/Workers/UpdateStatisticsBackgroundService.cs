using KL.Statistics.Application.Handlers;
using KL.Statistics.Configurations;
using Microsoft.Extensions.Options;

namespace KL.Statistics.Workers;

public class UpdateStatisticsBackgroundService : BackgroundService
{
    private readonly ILogger<DashboardPerformanceBackgroundService> _logger;
    private readonly BackgroundWorkerOptions _backgroundWorkerOptions;
    private readonly IServiceProvider _serviceProvider;

    public UpdateStatisticsBackgroundService(
        ILogger<DashboardPerformanceBackgroundService> logger,
        IOptions<BackgroundWorkerOptions> backgroundWorkerOptions,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _backgroundWorkerOptions = backgroundWorkerOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("{Service} Starting", nameof(DashboardPerformanceBackgroundService));

                await using var scope = _serviceProvider.CreateAsyncScope();
                var service = scope.ServiceProvider.GetRequiredService<ICdrUpdatedService>();

                await service.Process(stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(_backgroundWorkerOptions.RunUpdateCacheStatisticsInterval), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during {Service} executing", nameof(DashboardPerformanceBackgroundService));
                await Task.Delay(TimeSpan.FromSeconds(_backgroundWorkerOptions.RunUpdateCacheStatisticsInterval), stoppingToken);
            }
        }
    }
}