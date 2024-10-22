using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Models;
using KL.Engine.Rule.Repositories;
using KL.Engine.Rule.RuleEngine;
using KL.Engine.Rule.Services.Contracts;

namespace KL.Engine.Rule.Services;

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
