using KL.Agent.API.Application.Enums;
using KL.Agent.API.Application.Models.Responses;
using KL.Agent.API.Application.Services;

namespace KL.Agent.API.Application.Handlers;

public class AgentChangeStatusRequestHandler : IAgentChangeStatusRequestHandler
{
    private readonly ILogger<AgentChangeStatusRequestHandler> _logger;
    private readonly IAgentStateService _agentStateService;

    public AgentChangeStatusRequestHandler(
        ILogger<AgentChangeStatusRequestHandler> logger,
        IAgentStateService agentStateService
        )
    {
        _logger = logger;
        _agentStateService = agentStateService;
    }

    public async Task<HubResponse> Handle(long clientId, long agentId, AgentStatusTypes status)
    {
        if (status is not (AgentStatusTypes.WaitingForTheCall or AgentStatusTypes.InBreak or AgentStatusTypes.Offline))
            return HubResponse.CreateError($"Can't change status to {status}", HubErrorCode.BadRequest);

        await _agentStateService.ChangeAgentStatus(agentId, clientId, status);

        return HubResponse.CreateSuccess();
    }
}
