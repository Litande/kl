using KL.Manager.API.Application.Handlers;
using KL.Manager.API.Application.Models.Requests;
using KL.Manager.API.Application.Models.Requests.CallRecords;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KL.Manager.API.Controllers;

[Route("calls")]
public class CallsController : ApiAuthorizeBase
{
    private readonly ICDRRepository _cdrRepository;
    private readonly IDownloadAudioRecordHandler _downloadAudioRecordHandler;
    private readonly ICallFinishByManagerHandler _callFinishHandler;

    public CallsController(
        ICDRRepository cdrRepository,
        ICallFinishByManagerHandler callFinishHandler,
        IDownloadAudioRecordHandler downloadAudioRecordHandler)
    {
        _cdrRepository = cdrRepository;
        _callFinishHandler = callFinishHandler;
        _downloadAudioRecordHandler = downloadAudioRecordHandler;
    }

    [HttpPost]
    public async Task<ActionResult> CallList(
        [FromBody] CDRFilterRequest? filterRequest,
        [FromQuery] PaginationRequest? pagination = null,
        CancellationToken ct = default)
    {
        var p = new CallSearchPaginationRequest().Create(pagination);
        var response = await _cdrRepository.ListCalls(CurrentClientId, p, filterRequest, ct);

        return Ok(response);
    }

    [HttpGet]
    [Route("{callId}/audio")]
    public async Task<ActionResult> CallAudioRecord(
        [FromRoute] long callId,
        CancellationToken ct = default)
    {
        var ifModSince = Request.GetTypedHeaders().IfModifiedSince;
        var response = await _downloadAudioRecordHandler
            .Handle(CurrentClientId, CurrentUserId, callId, ifModSince, ct);

        if (response?.AudiRecord is null)
            return NotFound();

        if (!response.IsModified)
            return StatusCode(StatusCodes.Status304NotModified);

        var record = response.AudiRecord;

        Response.Headers.LastModified = record.LastModifiedAt.ToString("r");
        return File(record.Stream, record.Format, record.FileName);
    }

    [HttpPost("{sessionId}/finish")]
    public async Task<ActionResult> FinishCall(
        [FromRoute] string sessionId,
        CancellationToken ct = default)
    {
        await _callFinishHandler.Handle(sessionId, ct);
        return Ok();
    }
}