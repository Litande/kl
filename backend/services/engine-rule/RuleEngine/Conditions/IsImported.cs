using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Extensions;
using KL.Engine.Rule.RuleEngine.Contracts;
using KL.Engine.Rule.RuleEngine.Enums;

namespace KL.Engine.Rule.RuleEngine.Conditions;

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
