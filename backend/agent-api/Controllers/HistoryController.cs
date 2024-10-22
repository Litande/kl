using KL.Agent.API.Application.Handlers;
using KL.Agent.API.Application.Models.Requests;
using KL.Agent.API.Persistent.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KL.Agent.API.Controllers;

[Route("history")]
public class HistoryController : ApiAuthorizeBase
{
    private readonly ICDRRepository _cdrRepository;
    private readonly IDownloadAudioRecordHandler _downloadAudioRecordHandler;

    public HistoryController(
        ICDRRepository cdrRepository,
        IDownloadAudioRecordHandler downloadAudioRecordHandler)
    {
        _cdrRepository = cdrRepository;
        _downloadAudioRecordHandler = downloadAudioRecordHandler;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllCDRHistory(
        [FromQuery] PaginationRequest? pagination = null,
        CancellationToken ct = default)
    {
        var p = new CDRHistoryPaginationRequest().Create(pagination);
        var response = await _cdrRepository.GetAllByUserId(CurrentClientId, CurrentUserId, p, ct);
        return Ok(response);
    }

    [HttpGet]
    [Route("{callId}/play")]
    public async Task<ActionResult> CallAudioRecord(
        [FromRoute] long callId,
        CancellationToken ct = default)
    {
        var ifModSince = Request.GetTypedHeaders().IfModifiedSince;
        var response = await _downloadAudioRecordHandler.Handle(CurrentClientId, CurrentUserId, callId, ifModSince, ct);
    
        if (response?.AudiRecord is null)
            return NotFound();
        
        if (!response.IsModified)
            return StatusCode(StatusCodes.Status304NotModified);

        var record = response.AudiRecord;
        
        Response.Headers.LastModified = record.LastModifiedAt.ToString("r");
        return File(record.Stream, record.Format, record.FileName);
    }

    [HttpGet("lead/{phoneNumber}")]
    public async Task<IActionResult> GetAgentHistoryByPhone(
        [FromRoute] string phoneNumber,
        [FromQuery] PaginationRequest? pagination = null,
        CancellationToken ct = default)
    {
        var p = new CDRAgentHistoryPaginationRequest().Create(pagination);
        var response = await _cdrRepository.GetAllByLeadPhone(CurrentClientId, phoneNumber, p, ct);
        return Ok(response);
    }
}