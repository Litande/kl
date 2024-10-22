using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Extensions;
using KL.Engine.Rule.RuleEngine.Contracts;
using KL.Engine.Rule.RuleEngine.Enums;

namespace KL.Engine.Rule.RuleEngine.Conditions;

public class PreviousStatusCondition : RuleConditionBase, IRuleCondition
{
    public PreviousStatusCondition() : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.Behavior,
            RuleGroupTypes.LeadScoring
        },
        new RuleConditionDescription(
            RulesCondition.PreviousStatus.ToString(),
            RulesCondition.PreviousStatus.ToDescription(),
            ConditionsCategory.CustomField.ToDescription(),
            GetComparisonOperations(new[] { ComparisonOperation.Is, ComparisonOperation.IsNot }),
            new RuleFieldDescription[]
            {
                new(1, RuleValueType.Select.ToDescription(), "status", LeadStatuses, true)
            }
        )
    )
    { }

    public override Task<EngineRule> Prepare(RuleGroupData data) //TODO check impl
    {
        ValidateFields(data);
        ValidateComparison(data);

        var param1Value = ParseLeadStatus(data.Fields![0], data.Name);

        var comparison = data.ComparisonOperation switch
        {
            ComparisonOperation.Is => "",
            ComparisonOperation.IsNot => "!",
            _ => null
        };

        var ruleName = GenerateUniqueRuleName(data.Name);
        var expression = $"{comparison}ConditionsHelper.IsPreviousStatus({LeadParam}, {(int)param1Value})";
        return Task.FromResult(new EngineRule { RuleName = ruleName, Expression = expression });
    }
}
