using KL.Manager.API.Application.Enums;
using KL.Manager.API.Application.Models.Requests.Agents;
using KL.Manager.API.Persistent.Entities;
using Microsoft.AspNetCore.Identity;

namespace KL.Manager.API.Application.Handlers.Agents;

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
            
        var isAgent = await _userManager.IsInRoleAsync(user, UserRoleTypes.Agent.ToString());
        if (!isAgent)
            throw new InvalidDataException($"Agent with id {agentId} not found");

        string token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
        if (!result.Succeeded)
            throw new ArgumentException(result.ToString());
    }
}
