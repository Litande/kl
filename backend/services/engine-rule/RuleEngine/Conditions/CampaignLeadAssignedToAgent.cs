using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Extensions;
using KL.Engine.Rule.RuleEngine.Contracts;
using KL.Engine.Rule.RuleEngine.Enums;

namespace KL.Engine.Rule.RuleEngine.Conditions;

public class CampaignLeadAssignedToAgent : RuleConditionBase
{
    public CampaignLeadAssignedToAgent() : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.Behavior,
            RuleGroupTypes.LeadScoring
        },
        new RuleConditionDescription(
                    RulesCondition.CampaignLeadAssignedToAgent.ToString(),
                    RulesCondition.CampaignLeadAssignedToAgent.ToDescription(),
                    ConditionsCategory.CustomField.ToDescription(),
                    GetComparisonOperations(new[] { ComparisonOperation.Is }),
                    new RuleFieldDescription[]
                    {
                        new(1, RuleValueType.Select.ToDescription(), "assign", BasicToggles, true)
                    }
               )
    )
    { }

    public override Task<EngineRule> Prepare(RuleGroupData data) //TODO check impl
    {
        ValidateFields(data);
        ValidateComparison(data);

        var param1Value = ParseBool(data.Fields![0], data.Name);
        
        var expression = $"{LeadParam}.AssignedAgentId != null == {param1Value.ToString().ToLowerInvariant()}";
        var ruleName = GenerateUniqueRuleName(data.Name);
        return Task.FromResult(new EngineRule { RuleName = ruleName, Expression = expression });
    }
}
