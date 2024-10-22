using KL.Caller.Leads.App;
using KL.Caller.Leads.Services.Contracts;
using Microsoft.Extensions.Options;

namespace KL.Caller.Leads;

public class LeadStatisticBackgroundService : BackgroundService
{
    private readonly ILogger<LeadStatisticBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly LeadStatisticProcessingOptions _options;

    public LeadStatisticBackgroundService(
        ILogger<LeadStatisticBackgroundService> logger,
        IServiceProvider serviceProvider,
        IOptions<LeadStatisticProcessingOptions> options)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await StartProcessingLeadStatistics(ct);
    }

    private async Task StartProcessingLeadStatistics(CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("{RuleProcessor} Starting", nameof(StartProcessingLeadStatistics));

                await using var scope = _serviceProvider.CreateAsyncScope();
                var service = scope.ServiceProvider.GetRequiredService<ILeadStatisticProcessing>();

                await service.ProcessAll(ct);

                await Task.Delay(TimeSpan.FromMinutes(_options.RunIntervalMinutes), ct);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during {RuleProcessor} executing", nameof(StartProcessingLeadStatistics));
                await Task.Delay(TimeSpan.FromMinutes(1), ct);
            }
        }
    }
}