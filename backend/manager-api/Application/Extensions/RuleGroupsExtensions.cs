using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Application.Models.Requests.RuleGroups;
using Plat4Me.DialClientApi.Application.Models.Responses.Rule;
using Plat4Me.DialClientApi.Application.Models.Responses.RuleGroups;
using Plat4Me.DialClientApi.Persistent.Entities;

namespace Plat4Me.DialClientApi.Application.Extensions;

public static class RuleGroupsExtensions
{
    public static IEnumerable<RuleResponse> ToRulesResponse(this ICollection<Rule>? rules)
        => rules?.Select(i => new RuleResponse(i.Id, i.Name, i.Status, i.Rules)) ??
           Enumerable.Empty<RuleResponse>();

    public static RuleGroup ToModel(
        this CreateRuleGroupRequest request,
        RuleGroupTypes groupType,
        long clientId)
    {
        return new RuleGroup
        {
            Id = 0,
            ClientId = clientId,
            Name = request.Name,
            GroupType = groupType,
            Status = request.Status
        };
    }

    public static RuleGroup ToModel(
        this RuleGroup ruleGroup,
        UpdateRuleGroupRequest request,
        RuleGroupTypes groupType)
    {
        ruleGroup.Name = request.Name;
        ruleGroup.GroupType = groupType;
        ruleGroup.Status = request.Status;

        return ruleGroup;
    }

    public static RuleGroupResponse ToResponse(this RuleGroup ruleGroup)
        => new(
            ruleGroup.Id,
            ruleGroup.Name,
            ruleGroup.Status,
            ruleGroup.Rules.ToRulesResponse());
}