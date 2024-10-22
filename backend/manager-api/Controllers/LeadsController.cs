using KL.Manager.API.Application.Enums;
using KL.Manager.API.Application.Handlers.Leads;
using KL.Manager.API.Application.Models.Requests;
using KL.Manager.API.Application.Models.Requests.Leads;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KL.Manager.API.Controllers;

[Route("leads")]
public class LeadsController : ApiAuthorizeBase
{
    private readonly ILeadRepository _leadRepository;
    private readonly ILeadsQueueQueryHandler _leadsQueueQueryHandler;
    private readonly ILeadInfoQueryHandler _leadInfoQueryHandler;

    public LeadsController(
        ILeadRepository leadRepository,
        ILeadsQueueQueryHandler leadsQueueQueryHandler,
        ILeadInfoQueryHandler leadInfoQueryHandler)
    {
        _leadRepository = leadRepository;
        _leadsQueueQueryHandler = leadsQueueQueryHandler;
        _leadInfoQueryHandler = leadInfoQueryHandler;
    }

    [HttpGet]
    [Route("queues")]
    public async Task<ActionResult> LeadsQueue()
    {
        var response = await _leadsQueueQueryHandler.Handle(CurrentClientId);
        return Ok(response);
    }

    [HttpGet]
    [Route("{leadId}/shortinfo")]
    public async Task<ActionResult> GetLeadInfo(
        [FromRoute] long leadId,
        CancellationToken ct)
    {
        var response = await _leadInfoQueryHandler.Handle(CurrentClientId, leadId, ct);
        return Ok(response);
    }

    [HttpPut]
    [Route("{leadId}/assignment")]
    public async Task<ActionResult> UpdateLeadAssignment(
        [FromRoute] long leadId,
        [FromBody] LeadAssignmentUpdateRequest request,
        CancellationToken ct = default)
    {
        var response = await _leadRepository
            .UpdateLeadAssignment(CurrentClientId, CurrentUserId, leadId, request.AssignedAgentId, ct);

        return Ok(response);
    }

    [HttpPut]
    [Route("{leadId}/status")]
    public async Task<ActionResult> UpdateLeadStatus(
        [FromRoute] long leadId,
        [FromBody] LeadStatusUpdateRequest request,
        CancellationToken ct = default)
    {
        if (!Enum.TryParse<LeadStatusTypes>(request.Status, true, out var status))
            return BadRequest();

        var response = await _leadRepository.UpdateLeadStatus(CurrentClientId, CurrentUserId, leadId, status, ct);
        return Ok(response);
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult> LeadList(
        [FromBody] LeadsFilterRequest? filterRequest,
        [FromQuery] PaginationRequest? pagination = null,
        CancellationToken ct = default)
    {
        var p = new LeadSearchPaginationRequest().Create(pagination);
        var response = await _leadRepository.SearchLeads(CurrentClientId, p, filterRequest, ct);
        return Ok(response);
    }
}
