using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Extensions;
using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Actions;

public abstract class RuleActionBase : IRuleAction
{
    public RuleActionDescription Description { get; private set; }
    public IReadOnlyCollection<RuleGroupTypes> AvailableFor { get; private set; }
    public string Key => Description.Name;

    internal RuleActionBase(IReadOnlyCollection<RuleGroupTypes> availableFor, RuleActionDescription description)
    {
        AvailableFor = availableFor;
        Description = description;
    }

    public abstract Task<IRuleActionExecutor> Create(RuleActionData data);

    protected static IReadOnlyCollection<LabelValue> LeadStatuses => _leadStatuses;
    protected static IReadOnlyCollection<LabelValue> Toggles => _basicToggles;
    protected static IReadOnlyCollection<LabelValue> TimeUnitsList => _timeUnits;
    protected static IReadOnlyCollection<LabelValue> LeadFields => _leadFields;

    protected static IDictionary<string, RuleValueType> LeadFieldTypes => _leadFields
        .ToDictionary(r => r.Value, r => DefineFieldType(r.Value));

    protected static IEnumerable<LabelValue> ActionOperationList(IEnumerable<ActionOperation> operations)
    {
        var items = operations
            .Select(operation => new LabelValue(
                operation.ToDescription(),
                operation.ToString()));

        return items;
    }

    private static readonly LabelValue[] _basicToggles =
    {
        new("on", "true"),
        new("off", "false"),
    };

    private static readonly LabelValue[] _leadStatuses = EnumExtensions.EnumToList<LeadStatusTypes>()
        .Select(r => new LabelValue(
            r.Key.ToDescription(),
            r.Key.ToString()))
        .ToArray();

    private static readonly LabelValue[] _leadFields =
    {
        new(nameof(TrackedLead.Score), nameof(TrackedLead.Score)),
        new(nameof(TrackedLead.LeadQueueId), nameof(TrackedLead.LeadQueueId)),
        new(nameof(TrackedLead.LeadPhone), nameof(TrackedLead.LeadPhone)),
        new(nameof(TrackedLead.FirstName), nameof(TrackedLead.FirstName)),
        new(nameof(TrackedLead.LastName), nameof(TrackedLead.LastName)),
        new(nameof(TrackedLead.RegistrationTime), nameof(TrackedLead.RegistrationTime)),
    };

    private static RuleValueType DefineFieldType(string fieldName)
    {
        return fieldName switch
        {
            nameof(TrackedLead.Score)
                or nameof(TrackedLead.LeadQueueId)
                or nameof(TrackedLead.LeadPhone)
                or nameof(TrackedLead.FirstName)
                or nameof(TrackedLead.LastName)
                or nameof(TrackedLead.RegistrationTime)
                => RuleValueType.String,
            _ => throw new ArgumentException("Field not implemented", nameof(fieldName))
        };
    }

    private static readonly LabelValue[] _timeUnits = EnumExtensions.EnumToList<TimeUnits>()
        .Select(r => new LabelValue(
            r.Key.ToDescription(),
            r.Key.ToString()))
        .ToArray();

    protected static void ValidateFields(RuleActionData data, int requiredMinFieldsCount = 1)
    {
        if (data.Fields is null || data.Fields.Count < requiredMinFieldsCount)
            throw new ArgumentException("Missing action parameter", data.Name);
    }

    protected static RuleValueData GetValueTypeValidated(RuleValueData? param, string ruleName,
        params RuleValueType[] types)
    {
        ValidateValue(param, ruleName);

        if (!types.Contains(param!.Type))
            throw new ArgumentException("Action parameter has invalid type", ruleName);

        return param;
    }

    protected static LeadStatusTypes ParseLeadStatus(RuleValueData? param, string ruleName)
    {
        var r = GetValueTypeValidated(param, ruleName, RuleValueType.String, RuleValueType.Select);

        if (!Enum.TryParse(r.Value, out LeadStatusTypes paramValue))
            throw new ArgumentException("Invalid parameter value", ruleName);

        return paramValue;
    }

    protected static long ParseLong(RuleValueData? param, string ruleName) =>
        ParseLong(param, ruleName, RuleValueType.Integer);

    protected static long ParseLong(RuleValueData? param, string ruleName, params RuleValueType[] types)
    {
        var r = GetValueTypeValidated(param, ruleName, types);

        if (!long.TryParse(r.Value, out var paramValue))
            throw new ArgumentException("Invalid parameter value", ruleName);

        return paramValue;
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

    private static void ValidateValue(RuleValueData? param, string ruleName)
    {
        if (string.IsNullOrWhiteSpace(param?.Value))
            throw new ArgumentException("Parameter value cannot be null", ruleName);
    }
}