using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Extensions;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;
using RulesEngine.Models;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Conditions;

public class IsFixedAssignedFeedback : RuleConditionBase, IRuleCondition
{
    public IsFixedAssignedFeedback() : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.Behavior,
        },
        new RuleConditionDescription(
            RulesCondition.IsFixedAssignedFeedback.ToString(),
            RulesCondition.IsFixedAssignedFeedback.ToDescription(),
            ConditionsCategory.CustomField.ToDescription(),
            null,
            new RuleFieldDescription[]
            {
                new(1, RuleValueType.Select.ToDescription(), "status", LeadStatuses, true)
            }
        )
    )
    { }

    public override Task<Rule> Prepare(RuleGroupData data)
    {
        ValidateFields(data);

        var param1Value = ParseLeadStatus(data.Fields![0], data.Name);

        var ruleName = GenerateUniqueRuleName(data.Name);
        var expression = $"{LeadParam}.Status == {(int)param1Value} && {LeadParam}.LastCallAgentId != null && {LeadParam}.RemindOn != null && {LeadParam}.AssignedAgentId == null";
        return Task.FromResult(new Rule { RuleName = ruleName, Expression = expression });
    }
}
