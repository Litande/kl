using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plat4Me.DialLeadCaller.Application.Services.Contracts;
using Plat4Me.DialLeadCaller.Infrastructure.App;

namespace Plat4Me.DialLeadCaller.Infrastructure;

public class DropRateBackgroundService : BackgroundService
{
    private readonly ILogger<CallerLoopBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly DropRateBackgroundOptions _options;

    public DropRateBackgroundService(
        ILogger<CallerLoopBackgroundService> logger,
        IServiceProvider serviceProvider,
        IOptions<DropRateBackgroundOptions> options)
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
                _logger.LogInformation("{Service} Starting", nameof(DropRateBackgroundService));

                await using var scope = _serviceProvider.CreateAsyncScope();
                var service = scope.ServiceProvider.GetRequiredService<IDropRateService>();

                await service.Process(ct);

                await Task.Delay(TimeSpan.FromSeconds(_options.CalculatePeriodSeconds), ct);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during {Service} executing", nameof(DropRateBackgroundService));
            }
        }
    }
}
