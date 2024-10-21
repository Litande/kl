using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.Repositories;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
using Plat4Me.DialRuleEngine.Application.RuleEngine.MicrosoftEngine;
using Plat4Me.DialRuleEngine.Application.Services.Contracts;

namespace Plat4Me.DialRuleEngine.Application.Services;

public class LeadQueueRuleService : ILeadQueueRuleService
{
    private readonly ILogger<LeadQueueRuleService> _logger;
    private readonly IEngineMapper _engineMapper;
    private readonly ILeadQueueRepository _leadQueueRepository;
    private readonly ISettingsRepository _settingsRepository;
    public LeadQueueRuleService(
        IEngineMapper engineMapper,
        ILeadQueueRepository leadQueueRepository,
        ISettingsRepository settingsRepository,
        ILogger<LeadQueueRuleService> logger
    )
    {
        _engineMapper = engineMapper;
        _leadQueueRepository = leadQueueRepository;
        _settingsRepository = settingsRepository;
        _logger = logger;
    }

    public async Task ValidateRules(long clientId, RuleGroupTypes ruleType, string rules)
    {
        var clientEngineMapper = new ClientEngineMapper(clientId, _settingsRepository, _engineMapper, _logger);
        await clientEngineMapper.ValidateRules(ruleType, rules);
    }

    public async Task<IEnumerable<RuleActionDescription>> GetActions(
        long clientId,
        RuleGroupTypes ruleType,
        CancellationToken ct = default)
    {
        var clientEngineMapper = new ClientEngineMapper(clientId, _settingsRepository, _engineMapper, _logger);
        var actions = await clientEngineMapper.GetActions(ruleType);

        foreach (var action in actions)
        {
            switch (action.Name)
            {
                case "MoveLeadToQueue":
                    var queues = await _leadQueueRepository.GetAllByClient(clientId, ct: ct);
                    foreach (var field in action.Fields)
                    {
                        field.Values =
                            queues.Select(i => new LabelValue(i.Name, i.Id.ToString()));
                    }

                    break;
            }
        }

        return actions;
    }

    public async Task<IEnumerable<RuleConditionDescription>> GetConditions(long clientId, RuleGroupTypes ruleType)
    {
        var clientEngineMapper = new ClientEngineMapper(clientId, _settingsRepository, _engineMapper, _logger);
        var conditions = await clientEngineMapper.GetConditions(ruleType);
        return conditions;
    }
}
