using System.Text.Json;
using System.Text.Json.Serialization;
using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Extensions;
using KL.Engine.Rule.Models;
using KL.Engine.Rule.RuleEngine.Actions;
using KL.Engine.Rule.RuleEngine.Conditions;
using KL.Engine.Rule.RuleEngine.Contracts;
using KL.Engine.Rule.RuleEngine.Enums;

namespace KL.Engine.Rule.RuleEngine.MicrosoftEngine;

public interface IEngineMapper
{
    Task<IReadOnlyCollection<EngineRule>> MapToEngineRule(IEnumerable<RuleDto> rules, RuleGroupTypes rulesType,
        string ruleActionProperty,
        Dictionary<string, IRuleCondition>? additionalConditions = null,
        Dictionary<string, IRuleAction>? additionalActions = null
    );
    Task ValidateRules(RuleGroupTypes ruleType, string rules,
        Dictionary<string, IRuleCondition>? additionalConditions = null,
        Dictionary<string, IRuleAction>? additionalActions = null);
    Task<IEnumerable<RuleActionDescription>> GetActions(RuleGroupTypes ruleType);
    Task<IEnumerable<RuleConditionDescription>> GetConditions(RuleGroupTypes ruleType);
}

public class MicrosoftEngineMapper : IEngineMapper
{
    private readonly ILogger<MicrosoftEngineMapper> _logger;
    private readonly Dictionary<RuleGroupTypes, Dictionary<string, IRuleCondition>> _conditions;
    private readonly Dictionary<RuleGroupTypes, Dictionary<string, IRuleAction>> _actions;

    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    public MicrosoftEngineMapper(ILogger<MicrosoftEngineMapper> logger)
    {
        _logger = logger;
        _conditions = PrepareConditions();
        _actions = PrepareActions();
    }

    public async Task<IReadOnlyCollection<EngineRule>> MapToEngineRule(
        IEnumerable<RuleDto> rules,
        RuleGroupTypes rulesType,
        string ruleActionProperty,
        Dictionary<string, IRuleCondition>? additionalConditions = null,
        Dictionary<string, IRuleAction>? additionalActions = null
    )
    {
        if (!_conditions.TryGetValue(rulesType, out var conditions) || !conditions.Any())
            throw new ArgumentException($"No conditions defined for this ruleType {rulesType}");

        if (!_actions.TryGetValue(rulesType, out var actions) || !actions.Any())
            throw new ArgumentException($"No actions defined for this ruleType {rulesType}");

        if (additionalConditions is not null)
        {
            conditions = conditions.ToDictionary(x => x.Key, x => x.Value);
            foreach (var condition in additionalConditions)
            {
                conditions[condition.Key] = condition.Value;
            }
        }

        if (additionalActions is not null)
        {
            actions = actions.ToDictionary(x => x.Key, x => x.Value);
            foreach (var action in additionalActions)
            {
                actions[action.Key] = action.Value;
            }
        }

        var result = await Task.WhenAll(rules.OrderBy(x => x.Ordinal)
            .Select(async r => await MapToRootInternalRule(r, conditions, actions, ruleActionProperty)));

        return result;
    }

    public async Task ValidateRules(RuleGroupTypes ruleType, string rules,
        Dictionary<string, IRuleCondition>? additionalConditions = null,
        Dictionary<string, IRuleAction>? additionalActions = null)
    {
        //TODO validate conditions/actions by ruleType
        await MapToEngineRule(new RuleDto[] {
                new(
                    QueueId: null,
                    Name: "RuleValidation",
                    rules,
                    Ordinal: 0
                )}, ruleType, ruleActionProperty: "fake", additionalConditions, additionalActions);
    }

    public Task<IEnumerable<RuleActionDescription>> GetActions(RuleGroupTypes ruleType)
    {
        var result = _actions[ruleType].Values
            .Select(x => x.Description);

        return Task.FromResult(result);
    }

    public Task<IEnumerable<RuleConditionDescription>> GetConditions(RuleGroupTypes ruleType)
    {
        var result = _conditions[ruleType].Values
            .Select(x => x.Description)
            .OrderBy(x => x.DisplayName)
            .AsEnumerable();

        return Task.FromResult(result);
    }

    private async Task<EngineRule> MapToRootInternalRule(
        RuleDto r,
        IReadOnlyDictionary<string, IRuleCondition> conditions,
        IReadOnlyDictionary<string, IRuleAction> actions,
        string ruleActionProperty)
    {
        var entry = JsonSerializer.Deserialize<RuleEntry>(r.Rules, JsonOptions);

        if (entry?.Combination is null || entry.Actions is null)
            throw new ArgumentException("Rules parsing failed");

        var rule = (await PrepareRule(entry.Combination, "root-" + Guid.NewGuid(), conditions)).First(); // should be at least one?
        var ruleActions = new List<IRuleActionExecutor>();

        foreach (var actionData in entry.Actions)
        {
            if (actions.TryGetValue(actionData.Name, out var actionImpl))
                ruleActions.Add(await actionImpl.Create(actionData));
            else
                throw new ArgumentException($"Action {actionData.Name} was not found");
        }

        rule.Properties = new Dictionary<string, object>
        {
            {ruleActionProperty, ruleActions}
        };

        return rule;
    }

    private async Task<IEnumerable<EngineRule>> PrepareRule(
        RuleCombinationData combination,
        string ruleName,
        IReadOnlyDictionary<string, IRuleCondition> conditions
    )
    {
        var result = new List<EngineRule>();
        switch (combination.Operator)
        {
            case RuleCombinationOperator.True:
                {
                    var rule = new EngineRule
                    {
                        RuleName = ruleName,
                        Expression = "true"
                    };
                    result.Add(rule);
                }
                break;
            case RuleCombinationOperator.False:
                {
                    var rule = new EngineRule
                    {
                        RuleName = ruleName,
                        Expression = "false"
                    };
                    result.Add(rule);
                }
                break;
            case RuleCombinationOperator.None:
                {
                    if (combination.Groups is null || combination.Groups.Count != 1)
                        throw new ArgumentException("Groups must contain only one group for `None` combination");

                    var grp = combination.Groups.First();
                    result.Add(await PrepareRule(grp, conditions));
                }
                break;
            case RuleCombinationOperator.Or:
            case RuleCombinationOperator.And:
                {
                    var rule = new EngineRule
                    {
                        RuleName = ruleName,
                        Operator = combination.Operator.ToString()
                    };
                    var subRules = new List<EngineRule>();
                    if (combination.Combination is not null)
                    {
                        foreach (var subComb in combination.Combination.Select((combination, idx) => (combination, idx)))
                        {
                            subRules.AddRange(await PrepareRule(subComb.combination, ruleName + '_' + subComb.idx, conditions));
                        }
                    }
                    if (combination.Groups is not null)
                    {
                        subRules.AddRange(await Task.WhenAll(
                            combination.Groups!.Select(async r => await PrepareRule(r, conditions)))
                        );
                    }
                    rule.Rules = subRules;
                    result.Add(rule);
                }
                break;
            default:
                throw new NotImplementedException($"RuleCombinationOperator {combination.Operator} is not implemented");
        }
        return result;
    }

    private async Task<EngineRule> PrepareRule(RuleGroupData grp, IReadOnlyDictionary<string, IRuleCondition> conditions)
    {
        if (!conditions.TryGetValue(grp.Name, out var condition))
            throw new ArgumentException("Condition not found", grp.Name);

        var result = await condition.Prepare(grp);
        _logger.LogTrace($"Gen exp = `{result.Expression}`");

        return result;
    }

    private Dictionary<RuleGroupTypes, Dictionary<string, IRuleCondition>> PrepareConditions()
    {
        var ruleTypeList = EnumExtensions.EnumToList<RuleGroupTypes>();
        var result = ruleTypeList.ToDictionary(x => x.Key, _ => new Dictionary<string, IRuleCondition>(StringComparer.OrdinalIgnoreCase));
        var conditionTypes = GetTypes<IRuleCondition>().ToList();
        foreach (var conditionType in conditionTypes)
        {
            if (Attribute.GetCustomAttribute(conditionType, typeof(GenericCondition)) is not null)
                continue;
            var condition = (IRuleCondition?)Activator.CreateInstance(conditionType);
            if (condition is null)
            {
                _logger.LogWarning("Can not create condition instance {conditionType}. Skip.", conditionType.ToString());
                continue;
            }

            foreach (var ruleType in condition.AvailableFor)
            {
                if (!result[ruleType].TryAdd(condition.Key, condition))
                {
                    _logger.LogWarning("Condition {condition} is already registered.", condition.Key);
                }
            }
        }
        return result;
    }

    private Dictionary<RuleGroupTypes, Dictionary<string, IRuleAction>> PrepareActions()
    {
        var ruleTypeList = EnumExtensions.EnumToList<RuleGroupTypes>();
        var result = ruleTypeList.ToDictionary(x => x.Key, _ => new Dictionary<string, IRuleAction>(StringComparer.OrdinalIgnoreCase));
        var actionTypes = GetTypes<IRuleAction>().ToList();

        foreach (var actionType in actionTypes)
        {
            var action = (IRuleAction?)Activator.CreateInstance(actionType);
            if (action is null)
            {
                _logger.LogWarning("Can not create action instance {actionType}. Skip.", actionType.ToString());
                continue;
            }

            foreach (var ruleType in action.AvailableFor)
            {
                if (!result[ruleType].TryAdd(action.Key, action))
                {
                    _logger.LogWarning("Action {action} is already registered.", action.Key);
                }
            }
        }
        return result;
    }

    private IEnumerable<Type> GetTypes<T>()
    {
        var type = typeof(T);
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);
    }
}
