using Microsoft.AspNetCore.Mvc;
using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Application.Extensions;
using Plat4Me.DialClientApi.Application.Models.Requests.RuleGroups;
using Plat4Me.DialClientApi.Application.Services.Interfaces;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialClientApi.Controllers;

[Route("rules")]
public class RuleGroupController : ApiAuthorizeBase
{
    private readonly IRuleGroupService _ruleGroupService;
    private readonly IRuleGroupRepository _ruleGroupRepository;

    public RuleGroupController(
        IRuleGroupService ruleGroupService,
        IRuleGroupRepository ruleGroupRepository)
    {
        _ruleGroupService = ruleGroupService;
        _ruleGroupRepository = ruleGroupRepository;
    }

    [HttpGet]
    [Route("{ruleType}/groups")]
    public async Task<ActionResult> Get(
        [FromRoute] DisplayRuleGroupTypes ruleType,
        CancellationToken ct = default)
    {
        var response = await _ruleGroupService.GetRuleGroups(CurrentClientId, ruleType.GetRuleGroupType(), ct);
        return Ok(response);
    }

    [HttpPost]
    [Route("{ruleType}/groups")]
    public async Task<ActionResult> Post(
        [FromBody] CreateRuleGroupRequest request,
        [FromRoute] DisplayRuleGroupTypes ruleType,
        CancellationToken ct = default)
    {
        var response = await _ruleGroupService.AddRuleGroup(CurrentClientId, request, ruleType.GetRuleGroupType(), ct);
        return Ok(response);
    }

    [HttpPut]
    [Route("{ruleType}/groups/{groupId}")]
    public async Task<ActionResult> Put(
        [FromBody] UpdateRuleGroupRequest request,
        [FromRoute] DisplayRuleGroupTypes ruleType,
        [FromRoute] long groupId,
        CancellationToken ct = default)
    {
        var response = await _ruleGroupService.UpdateRuleGroup(CurrentClientId, request, groupId, ruleType.GetRuleGroupType(), ct);
        return Ok(response);
    }

    [HttpPut]
    [Route("{ruleType}/groups/{groupId}/{newStatus}")]
    public async Task<ActionResult> UpdateStatus(
        [FromRoute] DisplayRuleGroupTypes ruleType,
        [FromRoute] long groupId,
        [FromRoute] StatusTypes newStatus,
        CancellationToken ct = default)
    {
        var response = await _ruleGroupService.UpdateRuleGroupStatus(CurrentClientId, newStatus, groupId, ct);
        return Ok(response);
    }

    [HttpDelete]
    [Route("{ruleType}/groups/{groupId}")]
    public async Task<IActionResult> Delete(
        [FromRoute] DisplayRuleGroupTypes ruleType,
        [FromRoute] long groupId,
        CancellationToken ct = default)
    {
        var response = await _ruleGroupRepository.Delete(groupId, ct);
        if (!response) return NotFound();

        return Ok();
    }
}
