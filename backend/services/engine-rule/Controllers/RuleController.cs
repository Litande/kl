using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Models.Responses;
using KL.Engine.Rule.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KL.Engine.Rule.Controllers;

[Route("rules")]
public class RuleController : BaseController
{
    private readonly ILeadQueueRuleService _leadQueueRule;

    public RuleController(ILeadQueueRuleService leadQueueRule)
    {
        _leadQueueRule = leadQueueRule;
    }

    [HttpPost]
    [Route("{ruleType}/validate")]
    public async Task<ActionResult> ValidateRules(
        [FromRoute] RuleGroupTypes ruleType,
        CancellationToken ct = default)
    {
        var rules = await new StreamReader(Request.Body).ReadToEndAsync();
        if (string.IsNullOrEmpty(rules))
            return BadRequest();

        try
        {
            await _leadQueueRule.ValidateRules(CurrentClientId, ruleType, rules);
            return Ok(new OperationResponse(true));
        }
        catch (Exception ex)
        {
            return Ok(new OperationResponse(false, ex.Message));
        }
    }

    [HttpGet]
    [Route("{ruleType}/conditions")]
    public async Task<ActionResult> GetConditions(
        [FromRoute] RuleGroupTypes ruleType,
        CancellationToken ct = default)
    {
        var conditions = await _leadQueueRule.GetConditions(CurrentClientId, ruleType);
        return Ok(conditions);
    }

    [HttpGet]
    [Route("{ruleType}/actions")]
    public async Task<ActionResult> GetActions(
        [FromRoute] RuleGroupTypes ruleType,
        CancellationToken ct = default)
    {
        var actions = await _leadQueueRule.GetActions(CurrentClientId, ruleType, ct);
        return Ok(actions);
    }
}
