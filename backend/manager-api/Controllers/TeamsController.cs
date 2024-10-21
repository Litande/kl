using Microsoft.AspNetCore.Mvc;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialClientApi.Controllers;

[Route("teams")]
public class TeamsController : ApiAuthorizeBase
{
    private readonly ITeamRepository _teamRepository;

    public TeamsController(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var response = await _teamRepository.GetAll(CurrentClientId, ct);

        return Ok(response);
    }

    [HttpGet("info")]
    public async Task<IActionResult> GetAllWithExtraInfo(CancellationToken ct)
    {
        var response = await _teamRepository.GetAllWithExtraInfo(CurrentClientId, ct);

        return Ok(response);
    }
}
