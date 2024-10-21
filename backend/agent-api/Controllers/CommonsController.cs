using Microsoft.AspNetCore.Mvc;
using Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialAgentApi.Controllers;

[Route("commons")]
public class CommonsController : ApiAuthorizeBase
{
    private readonly ICommonsRepository _commons;

    public CommonsController(ICommonsRepository commonsRepository)
    {
        _commons = commonsRepository;
    }

    [HttpGet]
    [Route("leads/statuses")]
    public ActionResult LeadStatusList()
    {
        return Ok(_commons.LeadStatuses);
    }
}
