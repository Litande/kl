using Microsoft.AspNetCore.Mvc;
using Plat4Me.DialAgentApi.Application.Models.Requests;
using Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialAgentApi.Controllers;

[Route("leads")]
public class LeadController : ApiAuthorizeBase
{
    private readonly ILeadRepository _leadRepository;
    private readonly ILeadCommentRepository _leadCommentRepository;

    public LeadController(
        ILeadRepository leadRepository,
        ILeadCommentRepository leadCommentRepository)
    {
        _leadRepository = leadRepository;
        _leadCommentRepository = leadCommentRepository;
    }

    [HttpGet("callbacks")]
    public async Task<IActionResult> GetAllFutureCallBacks(
        [FromQuery] PaginationRequest? pagination = null,
        CancellationToken ct = default)
    {
        var p = new FutureCallBackPaginationRequest().Create(pagination);
        var response = await _leadRepository.GetAllFeatureCallBacks(CurrentClientId, CurrentUserId, p, ct);
        return Ok(response);
    }

    [HttpGet("{leadId:long}/comments")]
    public async Task<IActionResult> GetLeadComments(
        [FromRoute] long leadId,
        [FromQuery] PaginationRequest? pagination = null,
        CancellationToken ct = default)
    {
        var p = new LeadCommentPaginationRequest().Create(pagination);
        var response = await _leadCommentRepository.GetLeadComments(leadId, p, ct);
        return Ok(response);
    }

    [HttpPost("{leadId:long}/comments")]
    public async Task<IActionResult> AdComment(
        [FromRoute] long leadId,
        [FromBody] AddCommentRequest request,
        CancellationToken ct = default)
    {
        var response = await _leadCommentRepository
            .AddComment(CurrentUserId, leadId, request.Comment, ct);
        return Ok(response);
    }
}