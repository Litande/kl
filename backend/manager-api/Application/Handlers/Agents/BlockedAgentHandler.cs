using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;
using Plat4Me.DialClientApi.Application.Configurations;
using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Application.Models.Messages.Agents;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialClientApi.Application.Handlers.Agents;

public class BlockedAgentHandler : IBlockedAgentHandler
{
    private readonly UserManager<IdentityUser<long>> _userManager;
    private readonly INatsPublisher _natsPublisher;
    private readonly NatsPubSubOptions _pubSubjects;
    private readonly IBlockedUserCacheRepository _blockedUserCacheRepository;

    public BlockedAgentHandler(
        UserManager<IdentityUser<long>> userManager,
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