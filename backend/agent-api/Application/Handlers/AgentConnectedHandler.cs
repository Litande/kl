using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;
using Plat4Me.DialAgentApi.Application.Configurations;
using Plat4Me.DialAgentApi.Application.Enums;
using Plat4Me.DialAgentApi.Application.Services;
using Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialAgentApi.Application.Handlers;

public class AgentConnectedHandler : IAgentConnectedHandler
{
    private readonly IAgentTimeoutService _agentTimeoutService;
    private readonly IUserRepository _userRepository;
    private readonly IAgentStateService _agentStateService;

    public AgentConnectedHandler(
        IAgentTimeoutService agentTimeoutService,
        IUserRepository userRepository,
        IAgentStateService agentStateService
        )
    {
        _agentTimeoutService = agentTimeoutService;
        _userRepository = userRepository;
        _agentStateService = agentStateService;
    }

    public async Task Handle(long clientId, long agentId)
    {
        _agentTimeoutService.TryCancelTimeout(AgentTimeoutTypes.ConnectionTimeout, agentId.ToString());

        await _userRepository.SetOfflineSince(clientId, agentId, offlineSince: null);
        
        await _agentStateService.AgentConnected(agentId, clientId);
    }
}
