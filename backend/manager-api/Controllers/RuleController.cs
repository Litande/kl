using Microsoft.AspNetCore.Mvc;
using Plat4Me.DialClientApi.Application.Models.Requests.Rule;
using System.Text.Json.Nodes;
using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Application.Extensions;
using Plat4Me.DialClientApi.Application.Services.Interfaces;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;
using Plat4Me.DialClientApi.Persistent.Entities.Projections;

namespace Plat4Me.DialClientApi.Controllers;

[Route("rules")]
public class RuleController : ApiAuthorizeBase
{
    private readonly IRuleService _ruleService;
    private readonly IRuleRepository _ruleRepository;

    public RuleController(
        IRuleService ruleService,
        IRuleRepository ruleRepository)
    {
        _ruleService = ruleService;
        _ruleRepository = ruleRepository;
    }

    [HttpGet]
    [Route("{ruleType}/groups/{groupId}/rules/{ruleId}")]
    public async Task<ActionResult> Get(
        [FromRoute] DisplayRuleGroupTypes ruleType,
        [FromRoute] long groupId,
        [FromRoute] long ruleId,
        CancellationToken ct = default)
    {
        var response = await _ruleService.GetRule(CurrentClientId, ruleId, ct);
        return Ok(response);
    }

    [HttpPost]
    [Route("{ruleType}/groups/{groupId}/rules/")]
    public async Task<ActionResult> Post(
        [FromBody] CreateRuleRequest request,
        [FromRoute] DisplayRuleGroupTypes ruleType,
        [FromRoute] long groupId,
        CancellationToken ct = default)
    {
        var response = await _ruleService.AddRule(CurrentClientId, request, ruleType.GetRuleGroupType(), groupId, ct);
        return Ok(response);
    }

    [HttpPut]
    [Route("{ruleType}/groups/{groupId}/rules/{ruleId}")]
    public async Task<ActionResult> Put(
        [FromBody] UpdateRuleRequest request,
        [FromRoute] DisplayRuleGroupTypes ruleType,
        [FromRoute] long groupId,
        [FromRoute] long ruleId,
        CancellationToken ct = default)
    {
        var response = await _ruleService.UpdateRule(CurrentClientId, request, ruleId, ruleType.GetRuleGroupType(), groupId, ct);
        return Ok(response);
    }

    [HttpPut]
    [Route("{ruleType}/groups/{groupId}/rules/{ruleId}/{newStatus}")]
    public async Task<ActionResult> UpdateStatus(
        [FromRoute] DisplayRuleGroupTypes ruleType,
        [FromRoute] long groupId,
        [FromRoute] long ruleId,
        [FromRoute] StatusTypes newStatus,
        CancellationToken ct = default)
    {
        var response = await _ruleService.UpdateRuleStatus(CurrentClientId, newStatus, ruleId, ct);
        return Ok(response);
    }

    [HttpGet]
    [Route("{ruleType}/conditions")]
    public async Task<IActionResult> GetConditions(
        [FromRoute] DisplayRuleGroupTypes ruleType,
        CancellationToken ct = default)
    {
        var response = await _ruleService.GetConditions(CurrentClientId, ruleType.GetRuleGroupType(), ct);
        return Ok(response);
    }

    [HttpPost]
    [Route("{ruleType}/validate")]
    public async Task<ActionResult> ValidateRule(
        [FromRoute] DisplayRuleGroupTypes ruleType,
        [FromBody] JsonObject rules,
        CancellationToken ct = default)
    {
        if (rules is null)
            return BadRequest();

        var response = await _ruleService.ValidateRule(CurrentClientId, ruleType.GetRuleGroupType(), rules.ToString(), ct);
        return Ok(response);
    }

    [HttpGet]
    [Route("{ruleType}/actions")]
    public async Task<IActionResult> GetActions(
        [FromRoute] DisplayRuleGroupTypes ruleType,
        CancellationToken ct = default)
    {
        var response = await _ruleService.GetActions(CurrentClientId, ruleType.GetRuleGroupType(), ct);
        return Ok(response);
    }

    [HttpGet]
    [Route("status")]
    public async Task<ActionResult> GetStatusRules(
        CancellationToken ct = default)
    {
        var response = await _ruleService.GetStatusRules(CurrentClientId, ct);
        return Ok(response);
    }

    [HttpPut]
    [Route("status")]
    public async Task<ActionResult> SetStatusRules(
        [FromBody] List<StatusRuleProjection> rules,
        CancellationToken ct = default)
    {
        await _ruleService.UpdateStatusRules(CurrentClientId, rules, ct);
        return Ok();
    }

    [HttpDelete]
    [Route("{ruleType}/groups/{groupId}/rules/{ruleId}")]
    public async Task<IActionResult> Delete(
        [FromRoute] DisplayRuleGroupTypes ruleType,
        [FromRoute] long groupId,
        [FromRoute] long ruleId,
        CancellationToken ct = default)
    {
        var response = await _ruleRepository.Delete(ruleId, ct);
        if (!response) return NotFound();

        return Ok();
    }
}
