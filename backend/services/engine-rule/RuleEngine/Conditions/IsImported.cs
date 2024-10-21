using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Extensions;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;
using RulesEngine.Models;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Conditions;

public class IsImportedCondition : RuleConditionBase, IRuleCondition
{
    public IsImportedCondition() : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.ForwardRules,
            RuleGroupTypes.LeadScoring
        },
        new RuleConditionDescription(
            RulesCondition.IsImported.ToString(),
            RulesCondition.IsImported.ToDescription(),
            ConditionsCategory.CustomField.ToDescription(),
            null,
            null
        )
    )
    { }

    public override Task<Rule> Prepare(RuleGroupData data)
    {
        var ruleName = GenerateUniqueRuleName(data.Name);
        var expression = $"{LeadParam}.SystemStatus == {(int)LeadSystemStatusTypes.Imported}";
        return Task.FromResult(new Rule { RuleName = ruleName, Expression = expression });
    }
}
