using KL.Agent.API.Application.Enums;
using KL.Agent.API.Application.Services;
using KL.Agent.API.Persistent.Repositories.Interfaces;

namespace KL.Agent.API.Application.Handlers;

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
