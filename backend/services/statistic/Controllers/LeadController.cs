using KL.Statistics.Application.Handlers.LeadStatistics;
using Microsoft.AspNetCore.Mvc;

namespace KL.Statistics.Controllers;

[Route("lead")]
public class LeadController : ApiAuthorizeBase
{
    private readonly ILeadStatisticsQueryHandler _leadStatisticsQueryHandler;

    public LeadController(ILeadStatisticsQueryHandler leadStatisticsQueryHandler)
    {
        _leadStatisticsQueryHandler = leadStatisticsQueryHandler;
    }

    [HttpGet]
    [Route("new_leads_statistics")]
    public async Task<IActionResult> NewLeadStatistics()
    {
        var response = await _leadStatisticsQueryHandler.Handle(CurrentClientId);
        return Ok(response);
    }
}