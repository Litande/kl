using KL.Manager.API.Application.Handlers.Leads;
using KL.Manager.API.Application.Models.Requests.LeadQueue;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KL.Manager.API.Controllers;

[Route("leadQueues")]
public class LeadsQueuesController : ApiAuthorizeBase
{
    private readonly ILeadQueueRepository _leadQueueRepository;
    private readonly ILeadQueueUpdateRatioHandler _updateRatioHandler;

    public LeadsQueuesController(
        ILeadQueueRepository leadQueueRepository,
        ILeadQueueUpdateRatioHandler updateRatioHandler)
    {
        _leadQueueRepository = leadQueueRepository;
        _updateRatioHandler = updateRatioHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var response = await _leadQueueRepository.GetAll(CurrentClientId, ct: ct);
        return Ok(response);
    }


    [HttpPut("{queueId}/ratio/{ratio}")]
    public async Task<IActionResult> UpdateQueueRatio(
        [FromRoute] long queueId,
        [FromRoute] double ratio,
        CancellationToken ct)
    {
        var response = await _updateRatioHandler.Handle(CurrentClientId, queueId, ratio, ct);
        return Ok(response);
    }

    [HttpPut("{leadQueueId}")]
    public async Task<IActionResult> UpdateQueue(
        [FromRoute] long leadQueueId,
        [FromBody] UpdateLeadQueueRequest request,
        CancellationToken ct)
    {
        await _leadQueueRepository.UpdateLeadQueue(CurrentClientId, leadQueueId, request, ct);
        return Ok();
    }
}