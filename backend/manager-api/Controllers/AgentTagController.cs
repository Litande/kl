using Microsoft.AspNetCore.Mvc;
using Plat4Me.DialClientApi.Application.Models.Requests;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialClientApi.Controllers;

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
