using KL.Manager.API.Application.Handlers.Leads;
using KL.Manager.API.Application.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace KL.Manager.API.Controllers;

[Route("queues")]
public class QueueController : ApiAuthorizeBase
{
    private readonly ILeadsQueueQueryHandler _leadsQueueQueryHandler;

    public QueueController(ILeadsQueueQueryHandler leadsQueueQueryHandler)
    {
        _leadsQueueQueryHandler = leadsQueueQueryHandler;
    }

    [HttpGet("{queueId}/leads")]
    public async Task<IActionResult> Get(
        [FromRoute] long queueId,
        [FromQuery] PaginationRequest? pagination = null,
        CancellationToken ct = default)
    {
        var p = new LeadQueuePaginationRequest().Create(pagination);
        var response = await _leadsQueueQueryHandler.Handle(CurrentClientId, queueId, p, ct);
        if (response is null)
            return NotFound($"Queue with {queueId} not found");
        return Ok(response);
    }
}