using System.Text.Json.Nodes;
using Plat4Me.DialClientApi.Application.Models.Requests.Rule;
using Plat4Me.DialClientApi.Application.Models.Responses.Rule;
using Plat4Me.DialClientApi.Persistent.Entities;

namespace Plat4Me.DialClientApi.Application.Extensions;

public static class RuleExtensions
{
    public static Rule ToModel(
        this CreateRuleRequest request,
        long ruleGroupId)
    {
        return new Rule
        {
            Id = 0,
            RuleGroupId = ruleGroupId,
            Name = request.Name,
            Status = request.Status,
            Rules = request.Rules.ToJsonString() //TODO: ????
        };
    }

    public static Rule ToModel(
        this Rule rule,
        UpdateRuleRequest request,
        long ruleGroupId)
    {
        rule.RuleGroupId = ruleGroupId;
        rule.Name = request.Name;
        rule.Status = request.Status;
        rule.Rules = request.Rules.ToJsonString(); //TODO: ????

        return rule;
    }

    public static RuleResponse ToResponse(this Rule rule)
        => new(rule.Id, rule.Name, rule.Status, JsonNode.Parse(rule.Rules));
}
