using KL.Manager.API.Application.Handlers.Leads;
using KL.Manager.API.Application.Models.Requests;
using KL.Manager.API.Application.Models.Requests.LeadBlacklists;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KL.Manager.API.Controllers;

[Route("blacklist")]
public class LeadBlacklistController : ApiAuthorizeBase
{
    private readonly ILeadBlacklistRepository _leadBlacklistRepository;
    private readonly IBlockLeadHandler _blockLeadHandler;

    public LeadBlacklistController(
        ILeadBlacklistRepository leadBlacklistRepository,
        IBlockLeadHandler blockLeadHandler)
    {
        _leadBlacklistRepository = leadBlacklistRepository;
        _blockLeadHandler = blockLeadHandler;
    }

    [HttpGet]
    [Route("")]
    public async Task<ActionResult> LeadBlackList(
        [FromQuery] PaginationRequest? pagination = null,
        CancellationToken ct = default)
    {
        var p = new LeadBlacklistPaginationRequest().Create(pagination);
        var response = await _leadBlacklistRepository.GetAll(CurrentClientId, p, ct);

        return Ok(response);
    }

    [HttpPost]
    [Route("{leadId}")]
    public async Task<ActionResult> Post(
        [FromRoute] long leadId,
        CancellationToken ct = default)
    {
        await _blockLeadHandler.Handle(CurrentClientId, CurrentUserId, leadId, ct);
        return Ok();
    }

    [HttpDelete]
    [Route("")]
    public async Task<ActionResult> Delete(
        LeadBlacklistDeleteRequest request,
        CancellationToken ct = default)
    {
        var response = await _leadBlacklistRepository.Delete(CurrentClientId, request.LeadIds, ct);
        return response
            ? Ok()
            : NotFound();
    }
}