using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Extensions;
using KL.Engine.Rule.RuleEngine.Contracts;
using KL.Engine.Rule.RuleEngine.Enums;

namespace KL.Engine.Rule.RuleEngine.Conditions;

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

    public override Task<EngineRule> Prepare(RuleGroupData data)
    {
        var ruleName = GenerateUniqueRuleName(data.Name);
        var expression = $"{LeadParam}.Status == {(int)LeadStatusTypes.NewLead}";
        return Task.FromResult(new EngineRule { RuleName = ruleName, Expression = expression });
    }
}
