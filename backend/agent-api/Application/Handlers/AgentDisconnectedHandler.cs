using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;
using Plat4Me.DialAgentApi.Application.Configurations;
using Plat4Me.DialAgentApi.Application.Enums;
using Plat4Me.DialAgentApi.Application.Models.Messages;
using Plat4Me.DialAgentApi.Application.Services;
using Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialAgentApi.Application.Handlers;

public class AgentDisconnectedHandler : IAgentDisconnectedHandler
{
    private readonly INatsPublisher _natsPublisher;
    private readonly NatsPubSubOptions _natsPubSubOptions;
    private readonly AgentHubOptions _agentHubOptions;
    private readonly IAgentTimeoutService _agentTimeoutService;
    private readonly ILogger<AgentDisconnectedHandler> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public AgentDisconnectedHandler(
        IOptions<NatsPubSubOptions> natsPubSubOptions,
        INatsPublisher natsPublisher,
        IAgentTimeoutService agentTimeoutService,
        IOptions<AgentHubOptions> agentHubOptions,
        ILogger<AgentDisconnectedHandler> logger,
        IServiceScopeFactory scopeFactory
        )
    {
        _natsPublisher = natsPublisher;
        _logger = logger;
        _scopeFactory = scopeFactory;
        _agentTimeoutService = agentTimeoutService;
        _natsPubSubOptions = natsPubSubOptions.Value;
        _agentHubOptions = agentHubOptions.Value;
    }

    public Task Handle(long clientId, long agentId)
    {
        async void OnTimeout()
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var agentStateService = scope.ServiceProvider.GetRequiredService<IAgentStateService>();
           
            await agentStateService.AgentDisconnected(agentId, clientId);
            await userRepository.SetOfflineSince(clientId, agentId, DateTimeOffset.UtcNow);
        }

        _agentTimeoutService.SetTimeout(AgentTimeoutTypes.ConnectionTimeout, agentId.ToString(),
            _agentHubOptions.DisconnectMsgDelay, OnTimeout);
        return Task.CompletedTask;
    }

    public async Task Handle()
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var agentCacheClient = scope.ServiceProvider.GetRequiredService<IAgentCacheRepository>();
        var agents = await agentCacheClient.GetAllAgents();
        foreach (var agent in agents)
        {
            await Handle(agent.Value.ClientId, agent.Key);
        }
    }
}
