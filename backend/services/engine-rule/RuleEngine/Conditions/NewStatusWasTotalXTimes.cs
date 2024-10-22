using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Extensions;
using KL.Engine.Rule.Models;
using KL.Engine.Rule.RuleEngine.Contracts;
using KL.Engine.Rule.RuleEngine.Enums;

namespace KL.Engine.Rule.RuleEngine.Conditions;

public class NewStatusWasTotalXTimes : RuleConditionBase
{
    public NewStatusWasTotalXTimes() : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.Behavior,
            RuleGroupTypes.LeadScoring
        },
        new RuleConditionDescription(
            RulesCondition.NewStatusWasTotalXTimes.ToString(),
            RulesCondition.NewStatusWasTotalXTimes.ToDescription(),
            ConditionsCategory.CustomField.ToDescription(),
            GetComparisonOperations(new[] {
                ComparisonOperation.Equal,
                ComparisonOperation.NotEqual,
                ComparisonOperation.MoreThan, ComparisonOperation.MoreThanEqual,
                ComparisonOperation.LessThan, ComparisonOperation.LessThanEqual,
             }),
            new RuleFieldDescription[]
            {
                new(1, RuleValueType.Integer.ToDescription(), "times", Array.Empty<LabelValue>(), true),
            }
        )
    )
    { }

    public override Task<EngineRule> Prepare(RuleGroupData data) //TODO check impl
    {
        ValidateFields(data);
        ValidateComparison(data);

        return PrepareWithSingleParam(data.Name, data.Fields![0], data.ComparisonOperation!.Value);
    }

    private static Task<EngineRule> PrepareWithSingleParam(string name, RuleValueData? param1, ComparisonOperation operation)
    {
        var param1Value = ParseInt(param1, name);
        var comparison = DefineComparison(operation);
        var expression = $"ConditionsHelper.LastStatusTimes({LeadParam}) {comparison} {param1Value}";

        var ruleName = GenerateUniqueRuleName(name);
        return Task.FromResult(new EngineRule { RuleName = ruleName, Expression = expression });
    }
}
