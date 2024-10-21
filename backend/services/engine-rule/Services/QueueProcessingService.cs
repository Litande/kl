using Microsoft.Extensions.Logging;
using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Repositories;
using Plat4Me.DialRuleEngine.Application.Services.Contracts;
using Plat4Me.DialRuleEngine.Application.RuleEngine;

namespace Plat4Me.DialRuleEngine.Application.Services;

public class QueueProcessingService : IQueueProcessingService
{
    private readonly ILogger<QueueProcessingService> _logger;
    private readonly IRuleRepository _ruleRepository;
    private readonly IRuleEngineProcessingService _ruleEngineProcessingService;

    public QueueProcessingService(
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
        var rules = await _ruleRepository.GetRulesByType(clientId, RuleGroupTypes.ForwardRules, ct);

        _logger.LogInformation("Start {name} with rules {rules} for client Id {clientId}",
            nameof(QueueProcessingService), string.Join(", ", rules.Select(r => r.Name)), clientId);

        await _ruleEngineProcessingService.Process(
            clientId,
            leads,
            rules,
            RuleGroupTypes.ForwardRules,
            ct);
    }
}
