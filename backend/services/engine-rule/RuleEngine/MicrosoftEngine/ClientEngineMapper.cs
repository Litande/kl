using Microsoft.Extensions.Logging;
using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Extensions;
using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Actions;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Conditions;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
using Plat4Me.DialRuleEngine.Application.Repositories;
using RulesEngine.Models;
using System.Text.Json;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.MicrosoftEngine;

public class ClientEngineMapper : IEngineMapper
{
    private readonly ILogger _logger;
    private readonly IEngineMapper _engineMapper;
    private readonly ISettingsRepository _settingsRepository;
    private readonly long _clientId;
    public ClientEngineMapper(
        long clientId,
        ISettingsRepository settingsRepository,
        IEngineMapper engineMapper,
        ILogger logger
    )
    {
        _clientId = clientId;
        _engineMapper = engineMapper;
        _settingsRepository = settingsRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Rule>> MapToEngineRule(
        IEnumerable<RuleDto> rules,
        RuleGroupTypes rulesType,
        string ruleActionProperty,
        Dictionary<string, IRuleCondition>? additionalConditions = null,
        Dictionary<string, IRuleAction>? additionalActions = null)
    {
        var genericConditions = await PrepareGenericConditions(_clientId, rulesType);
        if (genericConditions is not null)
        {
            if (additionalConditions is null)
                additionalConditions = genericConditions;
            else
                foreach (var condition in additionalConditions)
                {
                    genericConditions[condition.Key] = condition.Value;
                }
        }

        return await _engineMapper.MapToEngineRule(rules, rulesType, ruleActionProperty, additionalConditions, additionalActions);
    }

    public async Task ValidateRules(RuleGroupTypes rulesType, string rules,
        Dictionary<string, IRuleCondition>? additionalConditions = null,
        Dictionary<string, IRuleAction>? additionalActions = null)
    {
        var genericConditions = await PrepareGenericConditions(_clientId, rulesType);
        if (genericConditions is not null)
        {
            if (additionalConditions is null)
                additionalConditions = genericConditions;
            else
                foreach (var condition in additionalConditions)
                {
                    genericConditions[condition.Key] = condition.Value;
                }
        }
        await _engineMapper.ValidateRules(rulesType, rules, additionalConditions, additionalActions);
    }

    public Task<IEnumerable<RuleActionDescription>> GetActions(RuleGroupTypes ruleType)
    {
        return _engineMapper.GetActions(ruleType);
    }

    public async Task<IEnumerable<RuleConditionDescription>> GetConditions(RuleGroupTypes ruleType)
    {
        var result = (await _engineMapper.GetConditions(ruleType)).ToList();
        var generics = await PrepareGenericConditions(_clientId, ruleType);
        if (generics is not null)
            result.AddRange(generics.Values
                .Select(x => x.Description)
                .OrderBy(x => x.DisplayName)
                .AsEnumerable()
            );
        return result.ToArray();
    }

    private void PrepareLeadFieldConditions(LeadFieldDescription[] fields, RuleGroupTypes rulesType, ref Dictionary<string, IRuleCondition> conditions)
    {
        foreach (var field in fields)
        {
            var condition = new LeadField(field);

            if (condition.AvailableFor.Contains(rulesType))
            {
                if (!conditions.TryAdd(condition.Key, condition))
                {
                    _logger.LogWarning("Condition {condition} is already registered.", condition.Key);
                }
            }
        }
    }

    private async Task<Dictionary<string, IRuleCondition>?> PrepareGenericConditions(long clientId, RuleGroupTypes rulesType)
    {
        var settingsJson = await _settingsRepository.GetValue(clientId, SettingTypes.RuleEngineConditions);
        if (settingsJson is null)
        {
            _logger.LogInformation("No RuleEngineConditions found for client {clientId}", clientId);
            return null;
        }
        var ruleTypeList = EnumExtensions.EnumToList<RuleGroupTypes>();
        var result = new Dictionary<string, IRuleCondition>();
        var settings = JsonSerializer.Deserialize<RuleEngineConditionsSettings>(settingsJson, MicrosoftEngineMapper.JsonOptions);
        if (settings is null)
            throw new ArgumentException($"Parsing failed: RuleEngineConditionsSettings of client {clientId}");

        if (settings.LeadFields is not null)
            PrepareLeadFieldConditions(settings.LeadFields, rulesType, ref result);

        return result;
    }
}
