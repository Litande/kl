using KL.Engine.Rule.App;
using KL.Engine.Rule.Services.Contracts;
using Microsoft.Extensions.Options;

namespace KL.Engine.Rule;

public class AgentAssignationExpiryBackgroundService : BackgroundService
{
    private readonly ILogger<AgentAssignationExpiryBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ExpirationAgentOptions _options;

    public AgentAssignationExpiryBackgroundService(
        ILogger<AgentAssignationExpiryBackgroundService> logger,
        IServiceProvider serviceProvider,
        IOptions<ExpirationAgentOptions> options)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _options = options.Value;
    }


    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await StartProcessing(ct);
    }

    private async Task StartProcessing(CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("{service} Starting", nameof(AgentAssignationExpiryBackgroundService));

                await using var scope = _serviceProvider.CreateAsyncScope();
                var service = scope.ServiceProvider.GetRequiredService<IAgentsAssignationExpiryProcessing>();

                await service.Process(_options.AgentsAssignationExpiresDays, ct);

                await Task.Delay(TimeSpan.FromMinutes(_options.RunIntervalMinutes), ct);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during {service} executing", nameof(AgentAssignationExpiryBackgroundService));

                await Task.Delay(TimeSpan.FromMinutes(1), ct);
            }
        }
    }
}