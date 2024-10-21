using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Extensions;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;
using RulesEngine.Models;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Conditions;

public class IsFutureCall : RuleConditionBase, IRuleCondition
{
    public IsFutureCall() : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.ForwardRules,
        },
        new RuleConditionDescription(
            RulesCondition.IsFutureCall.ToString(),
            RulesCondition.IsFutureCall.ToDescription(),
            ConditionsCategory.CustomField.ToDescription(),
            null,
            null
        )
    )
    { }

    public override Task<Rule> Prepare(RuleGroupData data)
    {
        var localParam1 = DateTimeOffset.UtcNow;

        var expression = $"{LeadParam}.RemindOn != null && {LeadParam}.RemindOn.Value.Date <= localParam1";
        return Task.FromResult(new Rule
        {
            RuleName = GenerateUniqueRuleName(data.Name),
            Expression = expression,
            LocalParams = new[] {
                new ScopedParam
                {
                    Name = "localParam1",
                    Expression = $"DateTimeOffset.Parse(\"{localParam1}\").Date",
                },
            }
        });
    }
}
