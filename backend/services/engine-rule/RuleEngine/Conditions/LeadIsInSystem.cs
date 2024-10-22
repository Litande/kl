using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Extensions;
using KL.Engine.Rule.Models;
using KL.Engine.Rule.RuleEngine.Contracts;
using KL.Engine.Rule.RuleEngine.Enums;

namespace KL.Engine.Rule.RuleEngine.Conditions;

public class LeadIsInSystemCondition : RuleConditionBase, IRuleCondition
{
    public LeadIsInSystemCondition() : base (
        new RuleGroupTypes[]
        {
            RuleGroupTypes.ForwardRules,
            RuleGroupTypes.LeadScoring
        },
        new RuleConditionDescription(
            RulesCondition.LeadIsInSystem.ToString(),
            RulesCondition.LeadIsInSystem.ToDescription(),
            ConditionsCategory.CustomField.ToDescription(),
            GetComparisonOperations(new[] { ComparisonOperation.Equal, ComparisonOperation.LessThan, ComparisonOperation.MoreThan }),
            new RuleFieldDescription[]
            {
                new(1, RuleValueType.Integer.ToDescription(), "value", Array.Empty<LabelValue>(), true),
                new(2, RuleValueType.Select.ToDescription(), "units", TimeUnitsList, true)
            }
        )
    )
    { }

    public override Task<Rule> Prepare(RuleGroupData data) //TODO check impl
    {
        ValidateFields(data, requiredMinFieldsCount: 2);
        ValidateComparison(data);

        var comparison = data.ComparisonOperation switch
        {
            ComparisonOperation.Equal => "==", //TODO ??? remove
            ComparisonOperation.MoreThan => "<",
            ComparisonOperation.LessThan => ">",
            _ => null
        };

        var param1Value = ParseInt(data.Fields![0], data.Name);
        var param2 = GetValueTypeValidated(data.Fields[1], data.Name, RuleValueType.String, RuleValueType.Select);

        if (!Enum.TryParse(param2.Value, true, out TimeUnits param2TimeUnits))
            throw new ArgumentException($"{data.Name}: Invalid parameter value");

        var param2TimeSpan = ConvertToTimeSpan(param2TimeUnits, param1Value, data.Name);
        var localParam1 = DateTimeOffset.UtcNow - param2TimeSpan;

        var ruleName = GenerateUniqueRuleName(data.Name);
        var expression = $"{LeadParam}.RegistrationTime {comparison} localParam1";
        return Task.FromResult(new Rule
        {
            RuleName = ruleName,
            Expression = expression,
            LocalParams = new[] {
                new ScopedParam
                {
                    Name = "localParam1",
                    Expression = $"DateTimeOffset.Parse(\"{localParam1}\")"
                }
            }
        });
    }
}
