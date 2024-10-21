﻿using Microsoft.Extensions.Options;
using Plat4Me.DialLeadProvider.Application.Configurations;
using Plat4Me.DialLeadProvider.Application.Services.Interfaces;

namespace Plat4Me.DialLeadProvider.Workers;

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