using System.Globalization;
using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Extensions;
using KL.Engine.Rule.Models;
using KL.Engine.Rule.Models.Entities;
using KL.Engine.Rule.RuleEngine.Contracts;
using KL.Engine.Rule.RuleEngine.Enums;
using KL.Engine.Rule.RuleEngine.MicrosoftEngine;

namespace KL.Engine.Rule.RuleEngine.Conditions;

[AttributeUsage(AttributeTargets.Class)]
public class GenericCondition : Attribute
{
    public GenericCondition(){}
}

public abstract class RuleConditionBase : IRuleCondition
{
    protected const string LeadParam = MicrosoftRuleEngineProcessingService.LeadInputName;
    protected const string CdrParam = MicrosoftRuleEngineProcessingService.CDRInputName;

    public RuleConditionDescription Description { get; protected set; }
    public IReadOnlyCollection<RuleGroupTypes> AvailableFor { get; protected set; }
    public virtual string Key => Description.Name;

    internal RuleConditionBase(IReadOnlyCollection<RuleGroupTypes> availableFor, RuleConditionDescription description)
    {
        AvailableFor = availableFor;
        Description = description;
    }

    public abstract Task<EngineRule> Prepare(RuleGroupData data);

    protected static string GenerateUniqueRuleName(string ruleName) =>
        ruleName + Guid.NewGuid();

    protected static void ValidateFields(RuleGroupData data, int requiredMinFieldsCount = 1)
    {
        if (data.Fields is null || data.Fields.Count < requiredMinFieldsCount)
            throw new ArgumentException("Missing condition parameter", data.Name);
    }

    protected static void ValidateComparison(RuleGroupData data)
    {
        if (data.ComparisonOperation is null)
            throw new ArgumentException("Comparison operation cannot be null", data.Name);
    }

    protected static int ParseInt(RuleValueData? param, string ruleName)
    {
        var r = GetValueTypeValidated(param, ruleName, RuleValueType.Integer);

        if (!int.TryParse(r.Value, out var paramValue))
            throw new ArgumentException("Invalid parameter value", ruleName);

        return paramValue;
    }

    protected static long ParseLong(RuleValueData? param, string ruleName)
    {
        var r = GetValueTypeValidated(param, ruleName, RuleValueType.Integer);

        if (!long.TryParse(r.Value, out var paramValue))
            throw new ArgumentException("Invalid parameter value", ruleName);

        return paramValue;
    }

    protected static bool ParseBool(RuleValueData? param, string ruleName)
    {
        var r = GetValueTypeValidated(param, ruleName, RuleValueType.Select);

        if (!bool.TryParse(r.Value, out var paramValue))
            throw new ArgumentException("Invalid parameter value", ruleName);

        return paramValue;
    }

    protected static IEnumerable<LeadStatusTypes> ParseLeadStatusList(RuleValueData? param, string ruleName)
    {
        var r = GetValueTypeValidated(param, ruleName, RuleValueType.SelectMultiItem);
        var values = r.Value!.Split(",", StringSplitOptions.RemoveEmptyEntries);

        foreach (var item in values)
            yield return ParseLeadStatus(item, ruleName);
    }

    protected static IEnumerable<string> ParseStringList(RuleValueData? param, string ruleName)
    {
        var r = GetValueTypeValidated(param, ruleName, RuleValueType.String, RuleValueType.SelectMultiItem, RuleValueType.StringSet);
        if (string.IsNullOrEmpty(r.Value)) return Enumerable.Empty<string>();
        var values = r.Value.Split(",");
        return values;
    }

    protected static IEnumerable<long> ParseIntegerList(RuleValueData? param, string ruleName)
    {
        var r = GetValueTypeValidated(param, ruleName,  RuleValueType.IntegerSet);
        var values = r.Value!.Split(",").Select(x => long.Parse(x));
        return values;
    }

    protected static LeadStatusTypes ParseLeadStatus(RuleValueData? param, string ruleName)
    {
        var r = GetValueTypeValidated(param, ruleName, RuleValueType.String, RuleValueType.Select);

        return ParseLeadStatus(r.Value!, ruleName);
    }

    protected static LeadStatusTypes ParseLeadStatus(string param, string ruleName)
    {
        if (!Enum.TryParse(param, out LeadStatusTypes paramValue))
            throw new ArgumentException("Invalid parameter value", ruleName);

        return paramValue;
    }

    protected static RuleValueData GetValueTypeValidated(RuleValueData? param, string ruleName, params RuleValueType[] types)
    {
        ValidateValue(param, ruleName);

        if (!types.Contains(param!.Type))
            throw new ArgumentException("Condition parameter has invalid type", ruleName);

        return param;
    }

    private static void ValidateValue(RuleValueData? param, string ruleName)
    {
        if (string.IsNullOrWhiteSpace(param?.Value))
            throw new ArgumentException("Parameter value cannot be null", ruleName);
    }

    protected static TimeUnits ConvertToTimeUnits(ComparisonOperation operation)
    {
        if (operation.IsHoursOperation()) return TimeUnits.Hours;
        if (operation.IsDaysOperation()) return TimeUnits.Days;

        throw new ArgumentOutOfRangeException(nameof(operation), operation, "Type not implemented");
    }

    protected static TimeSpan ConvertToTimeSpan(TimeUnits units, long value, string ruleName)
    {
        var r = units switch
        {
            TimeUnits.Minutes => TimeSpan.FromMinutes(value),
            TimeUnits.Hours => TimeSpan.FromHours(value),
            TimeUnits.Days => TimeSpan.FromDays(value),
            _ => throw new ArgumentException("Invalid action operation", ruleName)
        };

        return r;
    }

    protected static string DefineComparison(ComparisonOperation comparisonOperation)
    {
        switch (comparisonOperation)
        {
            case ComparisonOperation.Equal:
            case ComparisonOperation.EqualForLastYHours:
            case ComparisonOperation.EqualForLastYDays:
                return "==";

            case ComparisonOperation.NotEqual:
            case ComparisonOperation.NotEqualForLastYHours:
            case ComparisonOperation.NotEqualForLastYDays:
                return "!=";

            case ComparisonOperation.MoreThan:
            case ComparisonOperation.MoreThanForLastYHours:
            case ComparisonOperation.MoreThanForLastYDays:
                return ">";

            case ComparisonOperation.LessThan:
            case ComparisonOperation.LessThanForLastYHours:
            case ComparisonOperation.LessThanForLastYDays:
                return "<";

            case ComparisonOperation.MoreThanEqual:
            case ComparisonOperation.MoreThanEqualForLastYHours:
            case ComparisonOperation.MoreThanEqualForLastYDays:
                return ">=";
            case ComparisonOperation.LessThanEqual:
            case ComparisonOperation.LessThanEqualForLastYHours:
            case ComparisonOperation.LessThanEqualForLastYDays:
                return "<=";

            default:
                throw new ArgumentOutOfRangeException(nameof(comparisonOperation), comparisonOperation, "Type not implemented");
        }
    }

    protected static IReadOnlyCollection<LabelValue> TimeUnitsList => _timeUnitsList;
    protected static IReadOnlyCollection<LabelValue> BasicToggles => _basicToggles;
    protected static IReadOnlyCollection<LabelValue> LeadStatuses => _leadStatuses;

    protected static IReadOnlyCollection<LabelValue> Countries
    {
        get
        {
            if (_countries is null)
            {
                var cultureList = new List<LabelValue>();
                var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);
                foreach (var culture in cultures)
                {
                    var region = new RegionInfo(culture.Name);
                    if (cultureList.Any(r => r.Value == region.TwoLetterISORegionName)) continue;

                    cultureList.Add(new LabelValue(region.EnglishName, region.TwoLetterISORegionName));
                }

                _countries = cultureList
                    .OrderBy(x => x.Label).ToArray();
            }
            return _countries;
        }
    }

    protected static IEnumerable<LabelValue> GetComparisonOperations(IEnumerable<ComparisonOperation> operations)
    {
        var items = operations
            .Select(operation => new LabelValue(
                operation.ToDescription(),
                operation.ToString()));

        return items;
    }

    private static LabelValue[] _countries = null!;
    private static LabelValue[] _leadStatuses = EnumExtensions.EnumToList<LeadStatusTypes>()
        .Select(r => new LabelValue(
            r.Key.ToDescription(),
            r.Key.ToString()))
        .ToArray();

    private static LabelValue[] _basicToggles = new LabelValue[]
        {
            new("on", "true"),
            new("off", "false"),
        };

    private static LabelValue[] _timeUnitsList = EnumExtensions.EnumToList<TimeUnits>()
            .Select(r => new LabelValue(
                r.Key.ToDescription(),
                r.Key.ToString()))
            .ToArray();

}
