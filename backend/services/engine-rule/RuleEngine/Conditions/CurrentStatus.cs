using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Extensions;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;
using RulesEngine.Models;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Conditions;

public class CurrentStatusCondition : RuleConditionBase
{
    public CurrentStatusCondition() : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.ForwardRules,
        },
        new RuleConditionDescription(
           RulesCondition.CurrentStatus.ToString(),
           RulesCondition.CurrentStatus.ToDescription(),
           ConditionsCategory.CustomField.ToDescription(),
           GetComparisonOperations(new[] { ComparisonOperation.Is, ComparisonOperation.IsNot }),
           new RuleFieldDescription[]
           {
                new(1, RuleValueType.SelectMultiItem.ToDescription(), "status", LeadStatuses, true)
           }
       )
    )
    { }

    public override Task<Rule> Prepare(RuleGroupData data) //TODO check impl
    {
        ValidateFields(data);
        ValidateComparison(data);

        var param1Values = ParseLeadStatusList(data.Fields![0], data.Name);
        var localParam1 = string.Join(",", param1Values.Select(r => $"{nameof(LeadStatusTypes)}.{r}"));

        var comparison = data.ComparisonOperation switch
        {
            ComparisonOperation.Is => "",
            ComparisonOperation.IsNot => "!",
            _ => null
        };

        var expression = $"{comparison}localParam1.Contains({LeadParam}.Status)";
        return Task.FromResult(new Rule
        {
            RuleName = GenerateUniqueRuleName(data.Name),
            Expression = expression,
            LocalParams = new[] {
                new ScopedParam
                {
                    Name = "localParam1",
                    Expression = $"new LeadStatusTypes[] {{ {localParam1} }}",
                },
            }
        });
    }
}
