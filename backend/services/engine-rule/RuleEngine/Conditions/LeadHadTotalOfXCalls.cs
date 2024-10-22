using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Extensions;
using KL.Engine.Rule.Models;
using KL.Engine.Rule.RuleEngine.Contracts;
using KL.Engine.Rule.RuleEngine.Enums;

namespace KL.Engine.Rule.RuleEngine.Conditions;

public class LeadHadTotalOfXCalls : RuleConditionBase
{
    public LeadHadTotalOfXCalls() : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.Behavior,
            RuleGroupTypes.LeadScoring
        },
        new RuleConditionDescription(
            RulesCondition.LeadHadTotalOfXCalls.ToString(),
            RulesCondition.LeadHadTotalOfXCalls.ToDescription(),
            ConditionsCategory.CustomField.ToDescription(),
            GetComparisonOperations(new[] {
                ComparisonOperation.Equal,
                ComparisonOperation.NotEqual,
                ComparisonOperation.MoreThan,
                ComparisonOperation.LessThan,
                ComparisonOperation.MoreThanEqual,
                ComparisonOperation.LessThanEqual,
            }),
            new RuleFieldDescription[]
            {
                new(1, RuleValueType.Integer.ToDescription(), "times", Array.Empty<LabelValue>(), true),
            }
        )
    )
    {  }

    public override Task<EngineRule> Prepare(RuleGroupData data)
    {
        ValidateFields(data);
        ValidateComparison(data);

        return PrepareWithSingleParam(data.Name, data.Fields![0], data.ComparisonOperation!.Value);
    }

    private static Task<EngineRule> PrepareWithSingleParam(string name, RuleValueData? param1, ComparisonOperation operation)
    {
        var param1Value = ParseInt(param1, name);
        var comparison = DefineComparison(operation);
        var ruleName = GenerateUniqueRuleName(name);
        var expression = $"ConditionsHelper.GetTotalCallsCount({LeadParam}, {CdrParam}) {comparison} {param1Value}";

        return Task.FromResult(new EngineRule { RuleName = ruleName, Expression = expression });
    }
}
