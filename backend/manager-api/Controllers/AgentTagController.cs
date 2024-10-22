using KL.Manager.API.Application.Models.Requests;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KL.Manager.API.Controllers;

[Route("agentTags")]
public class AgentTagController : ApiAuthorizeBase
{
    private readonly IUserRepository _userRepository;

    public AgentTagController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] IReadOnlyCollection<long>? filterByTagIds = null,
        [FromQuery] PaginationRequest? pagination = null,
        CancellationToken ct = default)
    {
        var p = new AgentTagsPaginationRequest().Create(pagination);
        var response = await _userRepository.GetByTagIds(CurrentClientId, p, filterByTagIds, ct);

        return Ok(response);
    }

    [HttpPut]
    [Route("{agentId}")]
    public async Task<IActionResult> Put(
        [FromRoute] long agentId,
        [FromBody] IReadOnlyCollection<long>? tagIds = null,
        CancellationToken ct = default)
    {
        var response = await _userRepository.UpdateTags(CurrentClientId, agentId, tagIds, ct);

        return Ok(response);
    }
}
