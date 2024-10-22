using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Extensions;
using KL.Engine.Rule.RuleEngine.Contracts;
using KL.Engine.Rule.RuleEngine.Enums;
using RulesEngine.Models;

namespace KL.Engine.Rule.RuleEngine.Conditions;

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

    public override Task<EngineRule> Prepare(RuleGroupData data)
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

        return Task.FromResult(new EngineRule
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
