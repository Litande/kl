using KL.Agent.API.Persistent.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KL.Agent.API.Controllers;

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
