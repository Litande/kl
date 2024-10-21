using Microsoft.Extensions.Logging;
using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Repositories;
using Plat4Me.DialRuleEngine.Application.RuleEngine;
using Plat4Me.DialRuleEngine.Application.Services.Contracts;

namespace Plat4Me.DialRuleEngine.Application.Services;

public class BehaviourProcessingService : IBehaviourProcessingService
{
    private readonly ILogger<BehaviourProcessingService> _logger;
    private readonly IRuleEngineProcessingService _ruleEngineProcessingService;
    private readonly IRuleRepository _ruleRepository;

    public BehaviourProcessingService(
        ILogger<BehaviourProcessingService> logger,
        IRuleEngineProcessingService ruleEngineProcessingService,
        IRuleRepository ruleRepository)
    {
        _logger = logger;
        _ruleEngineProcessingService = ruleEngineProcessingService;
        _ruleRepository = ruleRepository;
    }

    public async Task Process(
        long clientId,
        IReadOnlyCollection<TrackedLead> leads,
        CancellationToken ct = default)
    {
        var rules = await _ruleRepository.GetRulesByType(clientId, RuleGroupTypes.Behavior, ct);

        _logger.LogInformation("Start {name} with rules {rules} for client Id {clientId}",
            nameof(BehaviourProcessingService), string.Join(", ", rules.Select(r => r.Name)), clientId);

        await _ruleEngineProcessingService.Process(
            clientId,
            leads,
            rules,
            RuleGroupTypes.Behavior,
            ct);
    }
}
