using KL.Manager.API.Application.Configurations;
using KL.Manager.API.Application.Enums;
using KL.Manager.API.Application.Models.Messages.Agents;
using KL.Manager.API.Persistent.Entities;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using KL.Nats;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace KL.Manager.API.Application.Handlers.Agents;

public class BlockedAgentHandler : IBlockedAgentHandler
{
    private readonly UserManager<User> _userManager;
    private readonly INatsPublisher _natsPublisher;
    private readonly NatsPubSubOptions _pubSubjects;
    private readonly IBlockedUserCacheRepository _blockedUserCacheRepository;

    public BlockedAgentHandler(
        UserManager<User> userManager,
        INatsPublisher natsPublisher,
        IOptions<NatsPubSubOptions> natsOptions,
        IBlockedUserCacheRepository blockedUserCacheRepository)
    {
        _userManager = userManager;
        _natsPublisher = natsPublisher;
        _blockedUserCacheRepository = blockedUserCacheRepository;
        _pubSubjects = natsOptions.Value;
    }

    public async Task Handle(long agentId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(agentId.ToString());
        var isAgent = await _userManager.IsInRoleAsync(user, UserRoleTypes.Agent.ToString());

        if (!isAgent)
            throw new InvalidDataException($"Agent with id {agentId} not found");

        const int countBlockedYears = 10;

        var lockoutEnd = DateTimeOffset.UtcNow.AddYears(countBlockedYears);
        await _userManager.SetLockoutEnabledAsync(user, true);
        await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);

        await _blockedUserCacheRepository.Add(agentId);

        await _natsPublisher.PublishAsync(_pubSubjects.AgentBlocked, new AgentBlockedMessage(agentId));
    }
}