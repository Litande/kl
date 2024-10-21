using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Extensions;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;
using RulesEngine.Models;
using System.Linq;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Conditions;

public class CountryCondition : RuleConditionBase
{
    public CountryCondition() : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.Behavior,
            RuleGroupTypes.LeadScoring,
            RuleGroupTypes.ApiRules,
            RuleGroupTypes.ForwardRules,
        },
        new RuleConditionDescription(
            RulesCondition.Country.ToString(),
            RulesCondition.Country.ToDescription(),
            ConditionsCategory.CustomField.ToDescription(),
            GetComparisonOperations(new[] { ComparisonOperation.Is, ComparisonOperation.IsNot }),
            new RuleFieldDescription[]
            {
                new(1, RuleValueType.SelectMultiItem.ToDescription(), "country", Countries, true)
            }
        )
    )
    { }

    public override Task<Rule> Prepare(RuleGroupData data)
    {
        ValidateFields(data);
        ValidateComparison(data);

        var param1Values = ParseStringList(data.Fields![0], data.Name);

        var localParam1 = string.Join(",", param1Values.Select(r => $"\"{r}\""));

        var comparison = data.ComparisonOperation switch
        {
            ComparisonOperation.Is => "",
            ComparisonOperation.IsNot => "!",
            _ => throw new ArgumentOutOfRangeException(nameof(data.ComparisonOperation), data.ComparisonOperation, "Type not implemented")
        };

        var expression = $"{comparison}localParam1.Contains({LeadParam}.CountryCode) ";

        return Task.FromResult(new Rule
        {
            RuleName = GenerateUniqueRuleName(data.Name),
            Expression = expression,
            LocalParams = new[] {
                new ScopedParam
                {
                    Name = "localParam1",
                    Expression = $"new string[] {{ {localParam1} }}",
                },
            }
        });
    }
}
