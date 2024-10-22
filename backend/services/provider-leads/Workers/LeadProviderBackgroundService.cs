using KL.Provider.Leads.Application.Configurations;
using KL.Provider.Leads.Application.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace KL.Provider.Leads.Workers;

public class LeadProviderBackgroundService : BackgroundService
{
    private readonly ILogger<LeadProviderBackgroundService> _logger;
    private readonly LeadProviderOptions _leadProviderOptions;
    private readonly IServiceProvider _serviceProvider;

    public LeadProviderBackgroundService(
        ILogger<LeadProviderBackgroundService> logger,
        IOptions<LeadProviderOptions> leadProviderOptions,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _leadProviderOptions = leadProviderOptions.Value;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("{Service} Starting", nameof(LeadProviderBackgroundService));

                await using var scope = _serviceProvider.CreateAsyncScope();
                var service = scope.ServiceProvider.GetRequiredService<ILeadsDataSourceSync>();

                await service.LeadsSync(stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(_leadProviderOptions.ReconnectIntervalMinutes), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during {Service} executing", nameof(LeadProviderBackgroundService));
                await Task.Delay(TimeSpan.FromMinutes(_leadProviderOptions.ReconnectIntervalMinutes), stoppingToken);
            }
        }
    }
}