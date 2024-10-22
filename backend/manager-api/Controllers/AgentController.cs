using KL.Manager.API.Application.Handlers.Agents;
using KL.Manager.API.Application.Models.Requests.Agents;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KL.Manager.API.Controllers;

[Route("agents")]
public class AgentController : ApiAuthorizeBase
{
    private readonly IUserRepository _userRepository;
    private readonly IBlockedAgentHandler _blockedAgentHandler;
    private readonly IAgentChangePasswordHandler _agentChangePasswordHandler;

    public AgentController(
        IUserRepository userRepository,
        IBlockedAgentHandler blockedAgentHandler,
        IAgentChangePasswordHandler agentChangePasswordHandler
        )
    {
        _userRepository = userRepository;
        _blockedAgentHandler = blockedAgentHandler;
        _agentChangePasswordHandler = agentChangePasswordHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] AgentsFilterRequest? filters = null,
        CancellationToken ct = default)
    {
        var response = await _userRepository.GetByTeams(CurrentClientId, filters, ct);

        return Ok(response);
    }

    [HttpGet]
    [Route("{agentId}")]
    public async Task<IActionResult> Get(
        [FromRoute] long agentId,
        CancellationToken ct = default)
    {
        var response = await _userRepository.GetWithTeamsTagsAndQueues(CurrentClientId, agentId, ct);
        if (response is null) return NotFound();

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post(
        [FromBody] CreateAgentRequest request,
        CancellationToken ct = default)
    {
        var response = await _userRepository.Create(CurrentClientId, CurrentUserId, request, ct);

        return Ok(response);
    }

    [HttpPut]
    [Route("{agentId}")]
    public async Task<IActionResult> Put(
        [FromRoute] long agentId,
        [FromBody] UpdateAgentRequest request,
        CancellationToken ct = default)
    {
        var response = await _userRepository.Update(CurrentClientId, agentId, request, ct);
        if (response is null) return NotFound();

        return Ok(response);
    }

    [HttpPost]
    [Route("{agentId}/block")]
    public async Task<IActionResult> BlockAgent(
        [FromRoute] long agentId,
        CancellationToken ct = default)
    {
        await _blockedAgentHandler.Handle(agentId, ct);
        return Ok();
    }

    [HttpPost]
    [Route("{agentId}/changepassword")]
    public async Task<IActionResult> ChangePassword(
        [FromRoute] long agentId,
        [FromBody] AgentChangePasswordRequest changePasswordRequest,
        CancellationToken ct = default)
    {
        await _agentChangePasswordHandler.Handle(agentId, changePasswordRequest, ct);
        return Ok();
    }
}
