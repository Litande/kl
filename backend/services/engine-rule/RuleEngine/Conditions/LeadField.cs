using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Extensions;
using KL.Engine.Rule.Models;
using KL.Engine.Rule.RuleEngine.Contracts;
using KL.Engine.Rule.RuleEngine.Enums;
using RulesEngine.Models;

namespace KL.Engine.Rule.RuleEngine.Conditions;

[GenericCondition]
public class LeadField : RuleConditionBase
{
    protected readonly LeadFieldDescription _fieldDescription;

    public override string Key => $"{typeof(LeadField).Name}_{_fieldDescription.Name}";

    public LeadField(LeadFieldDescription fieldDescription) : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.ForwardRules,
        },
        null!
    )
    {
        _fieldDescription = fieldDescription;
        Description = new RuleConditionDescription(
            Key,
            _fieldDescription.DisplayName,
            ConditionsCategory.LeadField.ToDescription(),
            GetComparisonOperations(_comparisonOperations),
            new RuleFieldDescription[]
            {
               new(1, _paramType.ToDescription(), "", _fieldDescription.SelectItems ?? Array.Empty<LabelValue>(), true)
            }
        );
    }

    protected IEnumerable<ComparisonOperation> _comparisonOperations =>
        _fieldDescription.Type switch
        {
            LeadFieldTypes.Identifier => new[] {
                    ComparisonOperation.Equal,
                    ComparisonOperation.NotEqual,
                    //ComparisonOperation.HasValue 
                }, //??? is/is not
            LeadFieldTypes.Integer => new[] {
                    //ComparisonOperation.HasValue,
                    ComparisonOperation.Equal, ComparisonOperation.NotEqual,
                    ComparisonOperation.MoreThan, ComparisonOperation.LessThan,
                    ComparisonOperation.MoreThanEqual, ComparisonOperation.LessThanEqual,
                },
            LeadFieldTypes.String or LeadFieldTypes.Set => new[] { 
                    //ComparisonOperation.HasValue,
                    ComparisonOperation.Equal, ComparisonOperation.NotEqual,
                    ComparisonOperation.Contains, ComparisonOperation.NotContains
                },
            _ => new ComparisonOperation[] { }
        };

    protected RuleValueType _paramType => _fieldDescription.Type switch
    {
        LeadFieldTypes.Identifier => RuleValueType.String,
        LeadFieldTypes.Integer => RuleValueType.Integer,
        LeadFieldTypes.String => RuleValueType.String,
        LeadFieldTypes.Set => _fieldDescription.SetElementType switch
        {
            LeadFieldTypes.Integer => RuleValueType.IntegerSet,
            LeadFieldTypes.String => RuleValueType.StringSet,
            LeadFieldTypes.Identifier => RuleValueType.StringSet,
            _ => throw new ArgumentException("Set should contain integer or string")
        },
        _ => throw new ArgumentException("Unknown field type")
    };

    public override Task<EngineRule> Prepare(RuleGroupData data)
    {
        ValidateFields(data);
        ValidateComparison(data);

        var ruleName = GenerateUniqueRuleName(data.Name);
        return Task.FromResult(_fieldDescription.Type switch
        {
            LeadFieldTypes.Identifier => PrepareIndentifierExpression(data, ruleName),
            LeadFieldTypes.String => PrepareStringExpression(data, ruleName),
            LeadFieldTypes.Integer => PrepareIntegerExpression(data, ruleName),
            LeadFieldTypes.Set => PrepareSetExpression(data, ruleName),
            _ => throw new ArgumentException("Unknown leadfield's type")
        });
    }


    private EngineRule PrepareIndentifierExpression(RuleGroupData data, string ruleName)
    {
        var comparison = data.ComparisonOperation switch
        {
            ComparisonOperation.Equal => data.ComparisonOperation,
            ComparisonOperation.NotEqual => data.ComparisonOperation,
            _ => throw new ArgumentException("Unknown leadfield's ComparisonOperation for identifier type")
        };

        return new EngineRule
        {
            RuleName = ruleName,
            Expression = $"ConditionsHelper.MetaDataStringValueCmp({LeadParam}, \"{_fieldDescription.Name}\", {(int)comparison}, \"{data.Fields![0].Value}\")"
        };
    }

    private EngineRule PrepareStringExpression(RuleGroupData data, string ruleName)
    {
        var comparison = data.ComparisonOperation switch
        {
            ComparisonOperation.Equal => data.ComparisonOperation,
            ComparisonOperation.NotEqual => data.ComparisonOperation,
            ComparisonOperation.Contains => data.ComparisonOperation,
            ComparisonOperation.NotContains => data.ComparisonOperation,
            _ => throw new ArgumentException("Unknown leadfield's ComparisonOperation for string type")
        };

        return new EngineRule
        {
            RuleName = ruleName,
            Expression = $"ConditionsHelper.MetaDataStringValueCmp({LeadParam}, \"{_fieldDescription.Name}\", {(int)comparison}, \"{data.Fields![0].Value}\")"
        };
    }

    private EngineRule PrepareIntegerExpression(RuleGroupData data, string ruleName)
    {
        var param1Value = ParseInt(data.Fields![0], data.Name);
        return new EngineRule
        {
            RuleName = ruleName,
            Expression = $"ConditionsHelper.MetaDataIntegerValueCmp({LeadParam}, \"{_fieldDescription.Name}\", {(int)data.ComparisonOperation!}, {param1Value})"
        };
    }

    private EngineRule PrepareSetExpression(RuleGroupData data, string ruleName)
    {
        switch (_fieldDescription.SetElementType)
        {
            case LeadFieldTypes.Integer:
                {
                    var param1Values = ParseIntegerList(data.Fields![0], data.Name);
                    var localParam1 = string.Join(",", param1Values.Select(r => r.ToString()));
                    return new EngineRule
                    {
                        RuleName = ruleName,
                        Expression = $"ConditionsHelper.MetaDataIntegerSetValueCmp({LeadParam}, \"{_fieldDescription.Name}\", {(int)data.ComparisonOperation!}, localParam1)",
                        LocalParams = new[] {
                            new ScopedParam
                            {
                                Name = "localParam1",
                                Expression = $"new long[] {{ {localParam1} }}",
                            },
                        }
                    };
                }
            case LeadFieldTypes.Identifier:
            case LeadFieldTypes.String:
                {
                    var param1Values = ParseStringList(data.Fields![0], data.Name);
                    var localParam1 = string.Join(",", param1Values.Select(r => $"\"{r}\""));
                    return new EngineRule
                    {
                        RuleName = ruleName,
                        Expression = $"ConditionsHelper.MetaDataStringSetValueCmp({LeadParam}, \"{_fieldDescription.Name}\", {(int)data.ComparisonOperation!}, localParam1)",
                        LocalParams = new[] {
                            new ScopedParam
                            {
                                Name = "localParam1",
                                Expression = $"new string[] {{ {localParam1} }}",
                            },
                        }
                    };
                }

            default:
                throw new ArgumentException("Unknown leadfield's set element type");
        }
    }


}
