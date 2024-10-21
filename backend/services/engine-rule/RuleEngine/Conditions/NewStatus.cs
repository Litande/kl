using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Extensions;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;
using RulesEngine.Models;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Conditions;

public class NewStatusCondition : RuleConditionBase
{
    public NewStatusCondition() : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.Behavior,
            RuleGroupTypes.LeadScoring
        },
        new RuleConditionDescription(
           RulesCondition.NewStatus.ToString(),
           RulesCondition.NewStatus.ToDescription(),
           ConditionsCategory.CustomField.ToDescription(),
           GetComparisonOperations(new[] { ComparisonOperation.Is, ComparisonOperation.IsNot }),
           new RuleFieldDescription[]
           {
                new(1, RuleValueType.Select.ToDescription(), "status", LeadStatuses, true)
           }
       )
    )
    { }

    public override Task<Rule> Prepare(RuleGroupData data) //TODO check impl
    {
        ValidateFields(data);
        ValidateComparison(data);

        var param1Value = ParseLeadStatus(data.Fields![0], data.Name);

        var comparison = data.ComparisonOperation switch
        {
            ComparisonOperation.Is => "==",
            ComparisonOperation.IsNot => "!=",
            _ => null
        };

        var ruleName = GenerateUniqueRuleName(data.Name);
        var expression = $"{LeadParam}.Status {comparison} {(int)param1Value}";
        return Task.FromResult(new Rule { RuleName = ruleName, Expression = expression });
    }
}
