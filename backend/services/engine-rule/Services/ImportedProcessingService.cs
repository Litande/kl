using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Models;
using KL.Engine.Rule.Repositories;
using KL.Engine.Rule.RuleEngine;
using KL.Engine.Rule.Services.Contracts;

namespace KL.Engine.Rule.Services;

public class ImportedProcessingService : IImportedProcessingService
{
    private readonly ILogger<QueueProcessingService> _logger;
    private readonly IRuleRepository _ruleRepository;
    private readonly IRuleEngineProcessingService _ruleEngineProcessingService;

    public ImportedProcessingService(
        ILogger<QueueProcessingService> logger,
        IRuleRepository ruleRepository,
        IRuleEngineProcessingService ruleEngineProcessingService)
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
        var rules = await _ruleRepository.GetRulesByType(clientId, RuleGroupTypes.ApiRules, ct);

        _logger.LogInformation("Start {name} with rules {rules} for client id {clientId}",
            nameof(ImportedProcessingService), string.Join(", ", rules.Select(r => r.Name)), clientId);

        await _ruleEngineProcessingService.Process(
            clientId,
            leads,
            rules,
            RuleGroupTypes.ApiRules,
            ct);
    }
}
