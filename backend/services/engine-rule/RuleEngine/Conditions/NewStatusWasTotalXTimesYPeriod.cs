using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Extensions;
using KL.Engine.Rule.Models;
using KL.Engine.Rule.RuleEngine.Contracts;
using KL.Engine.Rule.RuleEngine.Enums;
using RulesEngine.Models;

namespace KL.Engine.Rule.RuleEngine.Conditions;

public class NewStatusWasTotalXTimesYPeriod : RuleConditionBase
{
    public NewStatusWasTotalXTimesYPeriod() : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.Behavior,
            RuleGroupTypes.LeadScoring
        },
        new RuleConditionDescription(
            RulesCondition.NewStatusWasTotalXTimesYPeriod.ToString(),
            RulesCondition.NewStatusWasTotalXTimesYPeriod.ToDescription(),
            ConditionsCategory.CustomField.ToDescription(),
            GetComparisonOperations(new[] {
                ComparisonOperation.EqualForLastYHours, ComparisonOperation.EqualForLastYDays,
                ComparisonOperation.NotEqualForLastYHours, ComparisonOperation.NotEqualForLastYDays,
                ComparisonOperation.MoreThanForLastYHours, ComparisonOperation.MoreThanForLastYDays,
                ComparisonOperation.MoreThanEqualForLastYHours, ComparisonOperation.MoreThanEqualForLastYDays,
                ComparisonOperation.LessThanForLastYHours, ComparisonOperation.LessThanForLastYDays,
                ComparisonOperation.LessThanEqualForLastYHours, ComparisonOperation.LessThanEqualForLastYDays
             }),
            new RuleFieldDescription[]
            {
                new(1, RuleValueType.Integer.ToDescription(), "times", Array.Empty<LabelValue>(), true),
                new(2, RuleValueType.Integer.ToDescription(), "last time" , Array.Empty<LabelValue>(), true) //TODO ? last time options
            }
        )
    )
    { }

    public override Task<EngineRule> Prepare(RuleGroupData data) //TODO check impl
    {
        ValidateFields(data, 2);
        ValidateComparison(data);

        return PrepareWithMultipleParams(data.Name, data.Fields![0], data.Fields[1], data.ComparisonOperation!.Value);
    }

    private static Task<EngineRule> PrepareWithMultipleParams(string name, RuleValueData? param1, RuleValueData? param2, ComparisonOperation operation)
    {
        var param1Value = ParseInt(param1, name);
        var param2Value = ParseInt(param2, name);
        var operationTimeUnits = ConvertToTimeUnits(operation);
        var param2TimeSpan = ConvertToTimeSpan(operationTimeUnits, param2Value, name);
        var comparison = DefineComparison(operation);

        var localParam1 = DateTimeOffset.UtcNow - param2TimeSpan;
        var expression = $"ConditionsHelper.LastStatusTimes({LeadParam}, localParam1) {comparison} {param1Value}";

        return Task.FromResult(new EngineRule
        {
            RuleName = GenerateUniqueRuleName(name),
            Expression = expression,
            LocalParams = new[] {
                new ScopedParam
                {
                    Name = "localParam1",
                    Expression = $"DateTimeOffset.Parse(\"{localParam1}\")",
                }
            }
        });
    }
}
