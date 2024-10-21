using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;
using Plat4Me.DialAgentApi.Application.Configurations;
using Plat4Me.DialAgentApi.Application.Enums;
using Plat4Me.DialAgentApi.Application.Models.Messages;
using Plat4Me.DialAgentApi.Application.Models.Responses;
using Plat4Me.DialAgentApi.Application.Services;

namespace Plat4Me.DialAgentApi.Application.Handlers;

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
