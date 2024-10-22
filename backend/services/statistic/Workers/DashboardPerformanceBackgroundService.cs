using KL.Statistics.Application.Handlers.Dashboard;
using KL.Statistics.Configurations;
using Microsoft.Extensions.Options;

namespace KL.Statistics.Workers;

public class DashboardPerformanceBackgroundService : BackgroundService
{
    private readonly ILogger<DashboardPerformanceBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly DashboardPerformanceOptions _options;

    public DashboardPerformanceBackgroundService(
        ILogger<DashboardPerformanceBackgroundService> logger,
        IServiceProvider serviceProvider,
        IOptions<DashboardPerformanceOptions> options)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("{Service} Starting", nameof(DashboardPerformanceBackgroundService));

                await using var scope = _serviceProvider.CreateAsyncScope();
                var service = scope.ServiceProvider.GetRequiredService<ISenderDashboardPerformanceHandler>();

                await service.Handle(ct);

                await Task.Delay(TimeSpan.FromMinutes(_options.RunIntervalsMinutes), ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during {Service} executing", nameof(DashboardPerformanceBackgroundService));
                await Task.Delay(TimeSpan.FromMinutes(_options.RunIntervalsMinutes), ct);
            }
        }
    }
}
