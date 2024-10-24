using KL.Agent.API.Application.Enums;
using KL.Agent.API.Application.Models.Requests;
using KL.Agent.API.Persistent.Entities;
using Microsoft.AspNetCore.Identity;

namespace KL.Agent.API.Application.Handlers;

public class AgentChangePasswordHandler : IAgentChangePasswordHandler
{
    private readonly ILogger<AgentChangePasswordHandler> _logger;
    private readonly UserManager<User> _userManager;

    public AgentChangePasswordHandler(
        ILogger<AgentChangePasswordHandler> logger,
        UserManager<User> userManager
    )
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async Task Handle(long agentId, AgentChangePasswordRequest request, CancellationToken ct = default)
    {
        if (!string.Equals(request.NewPassword, request.ConfirmPassword))
            throw new ArgumentException("Passwords do not match");

        var user = await _userManager.FindByIdAsync(agentId.ToString());
        if (user is null)
            throw new InvalidDataException($"Agent with id {agentId} not found");

        var isAgent = await _userManager.IsInRoleAsync(user, RoleTypes.Agent.ToString());
        if (!isAgent)
            throw new InvalidDataException($"Agent with id {agentId} not found");

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
            throw new ArgumentException(result.ToString());
    }
}
