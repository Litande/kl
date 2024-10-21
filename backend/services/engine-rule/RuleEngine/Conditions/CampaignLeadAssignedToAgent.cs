using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Extensions;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;
using RulesEngine.Models;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Conditions;

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

    public override Task<Rule> Prepare(RuleGroupData data) //TODO check impl
    {
        ValidateFields(data);
        ValidateComparison(data);

        var param1Value = ParseBool(data.Fields![0], data.Name);
        
        var expression = $"{LeadParam}.AssignedAgentId != null == {param1Value.ToString().ToLowerInvariant()}";
        var ruleName = GenerateUniqueRuleName(data.Name);
        return Task.FromResult(new Rule { RuleName = ruleName, Expression = expression });
    }
}
