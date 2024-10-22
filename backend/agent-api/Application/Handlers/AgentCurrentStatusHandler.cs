using KL.Agent.API.Application.Enums;
using KL.Agent.API.Application.Services;

namespace KL.Agent.API.Application.Handlers;

public class AgentCurrentStatusHandler : IAgentCurrentStatusHandler
{
    private readonly IHubSender _hubSender;
    private readonly ILogger<AgentCurrentStatusHandler> _logger;

    public AgentCurrentStatusHandler(
        IHubSender hubSender,
        ILogger<AgentCurrentStatusHandler> logger)
    {
        _hubSender = hubSender;
        _logger = logger;
    }

    public async Task Handle(long agentId, AgentStatusTypes status, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Agent current status message - agentId: {AgentId}, status: {Status}",
                 agentId, status);

            await _hubSender.SendCurrentStatus(agentId.ToString(), status);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during {Service} executing", nameof(CallInfoHandler));
            throw;
        }
    }
}
