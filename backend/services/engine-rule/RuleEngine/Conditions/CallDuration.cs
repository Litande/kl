using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Extensions;
using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;
using RulesEngine.Models;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Conditions;

public class CallDuration : RuleConditionBase
{
    public CallDuration() : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.Behavior,
            RuleGroupTypes.LeadScoring
        },
        new RuleConditionDescription(
            RulesCondition.CallDuration.ToString(),
            RulesCondition.CallDuration.ToDescription(),
            ConditionsCategory.CustomField.ToDescription(),
            GetComparisonOperations(new[] { ComparisonOperation.Equal, 
                ComparisonOperation.LessThan, ComparisonOperation.LessThanEqual,
                ComparisonOperation.MoreThan, ComparisonOperation.MoreThanEqual }),
            new RuleFieldDescription[]
            {
                new(1, RuleValueType.Integer.ToDescription(), "seconds", Array.Empty<LabelValue>(), true)
            }
        )
    )
    { }

    public override Task<Rule> Prepare(RuleGroupData data) //TODO check impl
    {
        ValidateFields(data);
        ValidateComparison(data);

        var param1Value = ParseInt(data.Fields![0], data.Name);

        var comparison = data.ComparisonOperation switch
        {
            ComparisonOperation.Equal => "==",
            ComparisonOperation.LessThan => "<",
            ComparisonOperation.LessThanEqual => "<=",
            ComparisonOperation.MoreThan => ">",
            ComparisonOperation.MoreThanEqual => ">=",
            _ => null
        };

        var ruleName = GenerateUniqueRuleName(data.Name);
        var expression = $"ConditionsHelper.GetTotalCallsSeconds({LeadParam}, {CdrParam}) {comparison} {param1Value}";
        return Task.FromResult(new Rule { RuleName = ruleName, Expression = expression });
    }
}
