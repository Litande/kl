using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Extensions;
using KL.Engine.Rule.RuleEngine.Contracts;
using KL.Engine.Rule.RuleEngine.Enums;

namespace KL.Engine.Rule.RuleEngine.Conditions;

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
