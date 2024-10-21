using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Extensions;
using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;
using RulesEngine.Models;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Conditions;

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

    public override Task<Rule> Prepare(RuleGroupData data) //TODO check impl
    {
        ValidateFields(data);
        ValidateComparison(data);

        return PrepareWithSingleParam(data.Name, data.Fields![0], data.ComparisonOperation!.Value);
    }

    private static Task<Rule> PrepareWithSingleParam(string name, RuleValueData? param1, ComparisonOperation operation)
    {
        var param1Value = ParseInt(param1, name);
        var comparison = DefineComparison(operation);
        var expression = $"ConditionsHelper.LastStatusTimes({LeadParam}) {comparison} {param1Value}";

        var ruleName = GenerateUniqueRuleName(name);
        return Task.FromResult(new Rule { RuleName = ruleName, Expression = expression });
    }
}
