using Microsoft.AspNetCore.Mvc;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialClientApi.Controllers;

[Route("commons")]
public class CommonsController : ApiAuthorizeBase
{
    private readonly ICommonsRepository _commons;
    public CommonsController(ICommonsRepository commonsRepository)
    {
        _commons = commonsRepository;
    }

    [HttpGet]
    [Route("countries")]
    public async Task<ActionResult> CountryList(
        CancellationToken ct = default)
    {
        return Ok(_commons.Countries);
    }

    [HttpGet]
    [Route("leads/statuses")]
    public async Task<ActionResult> LeadStatusList(
        CancellationToken ct = default)
    {
        return Ok(_commons.LeadStatuses);
    }
}
