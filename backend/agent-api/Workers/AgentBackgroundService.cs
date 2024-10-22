using KL.Agent.API.Application.Handlers;

namespace KL.Agent.API.Workers;

public class AgentBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AgentBackgroundService> _logger;

    public AgentBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<AgentBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await RestoreAgentsTimeouts();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during {Service} executing", nameof(AgentBackgroundService));
            throw;
        }
    }

    private async Task RestoreAgentsTimeouts()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var feedbackTimeoutHandler = scope.ServiceProvider.GetRequiredService<IFeedbackTimeoutHandler>();
        await feedbackTimeoutHandler.Handle();

        var agentDisconnectedHandler = scope.ServiceProvider.GetRequiredService<IAgentDisconnectedHandler>();
        await agentDisconnectedHandler.Handle();
    }
}
