using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Extensions;
using KL.Engine.Rule.Models;
using KL.Engine.Rule.RuleEngine.Contracts;
using KL.Engine.Rule.RuleEngine.Enums;

namespace KL.Engine.Rule.RuleEngine.Conditions;

public class CampaignLeadWeightIsCondition : RuleConditionBase
{
    public CampaignLeadWeightIsCondition() : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.Behavior,
            RuleGroupTypes.LeadScoring,
            RuleGroupTypes.ForwardRules
        },
        new RuleConditionDescription(
            RulesCondition.CampaignLeadWeightIs.ToString(),
            RulesCondition.CampaignLeadWeightIs.ToDescription(),
            ConditionsCategory.CustomField.ToDescription(),
            GetComparisonOperations(new[] {
                ComparisonOperation.Equal,
                ComparisonOperation.NotEqual,
                ComparisonOperation.MoreThan,
                ComparisonOperation.LessThan,
                ComparisonOperation.LessThanEqual,
                ComparisonOperation.MoreThanEqual
            }),
            new RuleFieldDescription[]
            {
                new(1, RuleValueType.Integer.ToDescription(), "weight", Array.Empty<LabelValue>(), true)
            }
        )
    )
    { }

    public override Task<EngineRule> Prepare(RuleGroupData data)
    {
        ValidateFields(data);
        ValidateComparison(data);

        var param1Value = ParseLong(data.Fields![0], data.Name);

        var comparison = data.ComparisonOperation switch
        {
            ComparisonOperation.Equal => "==",
            ComparisonOperation.NotEqual => "!=",
            ComparisonOperation.MoreThan => ">",
            ComparisonOperation.LessThan => "<",
            ComparisonOperation.MoreThanEqual => ">=",
            ComparisonOperation.LessThanEqual => "<=",
            _ => throw new ArgumentOutOfRangeException(nameof(data.ComparisonOperation), data.ComparisonOperation, "Type not implemented")
        };

        var ruleName = GenerateUniqueRuleName(data.Name);
        var expression = $"{LeadParam}.Score {comparison} {param1Value}";
        return Task.FromResult(new EngineRule { RuleName = ruleName, Expression = expression });
    }
}
