using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Extensions;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;
using RulesEngine.Models;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Conditions;

public class IsTestLeadCondition : RuleConditionBase, IRuleCondition
{
    public IsTestLeadCondition() : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.ForwardRules,
            RuleGroupTypes.LeadScoring,
            RuleGroupTypes.Behavior,
        },
        new RuleConditionDescription(
            RulesCondition.IsTestLead.ToString(),
            RulesCondition.IsTestLead.ToDescription(),
            ConditionsCategory.CustomField.ToDescription(),
            GetComparisonOperations(new[] { ComparisonOperation.Is, ComparisonOperation.IsNot  }),
            null
        )
    )
    { }

    public override Task<Rule> Prepare(RuleGroupData data)
    {
        ValidateComparison(data);

        var comparison = data.ComparisonOperation switch
        {
            ComparisonOperation.Is => "",
            ComparisonOperation.IsNot => "!",
            _ => null
        };

        var ruleName = GenerateUniqueRuleName(data.Name);
        var expression = $"{comparison}{LeadParam}.IsTest";
        return Task.FromResult(new Rule { RuleName = ruleName, Expression = expression });
    }
}
