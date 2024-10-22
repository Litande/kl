using KL.Engine.Rule.Handlers.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KL.Engine.Rule.Controllers;

[Route("lead-queue")]
public class LeadQueueController : BaseController
{
    private readonly IGetNextFromLeadQueueHandler _getNextFromLeadQueueHandler;
    private readonly ISetLeadManualScoreHandler _setLeadManualScoreHandler;

    public LeadQueueController(
        IGetNextFromLeadQueueHandler getNextFromLeadQueueHandler,
        ISetLeadManualScoreHandler setLeadManualScoreHandler)
    {
        _getNextFromLeadQueueHandler = getNextFromLeadQueueHandler;
        _setLeadManualScoreHandler = setLeadManualScoreHandler;
    }

    [HttpPut("{queueId}/get-next")]
    public async Task<IActionResult> Pop(
        [FromRoute] long queueId,
        [FromBody] IReadOnlyCollection<long>? agentIds,
        CancellationToken ct)
    {
        var result = await _getNextFromLeadQueueHandler.Process(CurrentClientId, queueId, agentIds, ct);
        return Ok(result);
    }

    [HttpPut("{queueId}/lead/{leadId}/set-score")]
    public async Task<IActionResult> SetScore(
        [FromRoute] long queueId,
        [FromRoute] long leadId,
        [FromQuery] long score,
        CancellationToken ct)
    {
        await _setLeadManualScoreHandler.Process(CurrentClientId, queueId, leadId, score, ct);
        return Ok();
    }
}
