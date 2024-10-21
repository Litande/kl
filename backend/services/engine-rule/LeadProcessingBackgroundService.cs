using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;
using Plat4Me.DialRuleEngine.Application.App;
using Plat4Me.DialRuleEngine.Application.Models.Messages;
using Plat4Me.DialRuleEngine.Application.Services.Contracts;
using Plat4Me.DialRuleEngine.Infrastructure.App;
using Plat4Me.DialRuleEngine.Application.Repositories;

namespace Plat4Me.DialRuleEngine.Infrastructure;

public class LeadProcessingBackgroundService : BackgroundService
{
    private readonly ILogger<LeadProcessingBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly LeadProcessingOptions _leadProcessingOptions;
    private readonly PubSubjects _pubSubjects;
    private readonly INatsPublisher _natsPublisher;

    public LeadProcessingBackgroundService(
        ILogger<LeadProcessingBackgroundService> logger,
        IServiceProvider serviceProvider,
        IOptions<LeadProcessingOptions> leadProcessingOptions,
        IOptions<PubSubjects> pubSubjects,
        INatsPublisher natsPublisher)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _leadProcessingOptions = leadProcessingOptions.Value;
        _pubSubjects = pubSubjects.Value;
        _natsPublisher = natsPublisher;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await _natsPublisher.PublishAsync(_pubSubjects.RuleEngineRun, new RuleEngineRunMessage());

        await ValidateCache(ct);

        await StartProcessing(ct);
    }

    private async Task StartProcessing(CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("{service} Starting", nameof(LeadProcessingBackgroundService));

                await using var scope = _serviceProvider.CreateAsyncScope();
                var service = scope.ServiceProvider.GetRequiredService<ILeadProcessingPipeline>();

                await service.Process(ct);

                await Task.Delay(TimeSpan.FromMinutes(_leadProcessingOptions.RunIntervalMinutes), ct);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during {service} executing", nameof(LeadProcessingBackgroundService));
                await Task.Delay(TimeSpan.FromMinutes(1), ct);
            }
        }
    }

    private async Task ValidateCache(CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("{service} Starting", nameof(LeadProcessingBackgroundService.ValidateCache));

            await using var scope = _serviceProvider.CreateAsyncScope();
            var leadRepository = scope.ServiceProvider.GetRequiredService<ILeadRepository>();
            var leads = await leadRepository.GetLeadsWithSystemStatus(ct);
            var queueCache = scope.ServiceProvider.GetRequiredService<IQueueLeadsCacheRepository>();
            await queueCache.ValidateLeadCache(leads, ct);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during {service} executing", nameof(LeadProcessingBackgroundService.ValidateCache));
            await Task.Delay(TimeSpan.FromMinutes(1), ct);
        }
    }
}
