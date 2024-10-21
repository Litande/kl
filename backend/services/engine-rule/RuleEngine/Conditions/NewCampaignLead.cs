using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Extensions;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;
using RulesEngine.Models;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Conditions;

public class NewCampaignLeadCondition : RuleConditionBase
{
    public NewCampaignLeadCondition() : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.Behavior,
            RuleGroupTypes.LeadScoring
        },
        new RuleConditionDescription(
            RulesCondition.NewCampaignLead.ToString(),
            RulesCondition.NewCampaignLead.ToDescription(),
            ConditionsCategory.CustomField.ToDescription(),
            null,
            null
        )
    )
    { }

    public override Task<Rule> Prepare(RuleGroupData data)
    {
        var ruleName = GenerateUniqueRuleName(data.Name);
        var expression = $"{LeadParam}.Status == {(int)LeadStatusTypes.NewLead}";
        return Task.FromResult(new Rule { RuleName = ruleName, Expression = expression });
    }
}
