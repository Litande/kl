using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Extensions;
using KL.Engine.Rule.RuleEngine.Contracts;
using KL.Engine.Rule.RuleEngine.Enums;

namespace KL.Engine.Rule.RuleEngine.Conditions;

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

    public override Task<EngineRule> Prepare(RuleGroupData data)
    {
        ValidateFields(data);

        var param1Value = ParseLeadStatus(data.Fields![0], data.Name);

        var ruleName = GenerateUniqueRuleName(data.Name);
        var expression = $"{LeadParam}.Status == {(int)param1Value} && {LeadParam}.LastCallAgentId != null && {LeadParam}.RemindOn != null && {LeadParam}.AssignedAgentId == null";
        return Task.FromResult(new EngineRule { RuleName = ruleName, Expression = expression });
    }
}
