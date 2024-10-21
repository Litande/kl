using Microsoft.Extensions.Logging;
using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Repositories;
using Plat4Me.DialRuleEngine.Application.Services.Contracts;
using Plat4Me.DialRuleEngine.Application.RuleEngine;

namespace Plat4Me.DialRuleEngine.Application.Services;

public class ScoreProcessingService : IScoreProcessingService
{
    private readonly ILogger<ScoreProcessingService> _logger;
    private readonly IRuleRepository _ruleRepository;
    private readonly IRuleEngineProcessingService _ruleEngineProcessingService;
    private readonly ILeadLastCacheRepository _leadLastCacheRepository;

    public ScoreProcessingService(
        ILogger<ScoreProcessingService> logger,
        IRuleRepository ruleRepository,
        IRuleEngineProcessingService ruleEngineProcessingService,
        ILeadLastCacheRepository leadLastCacheRepository)
    {
        _logger = logger;
        _ruleRepository = ruleRepository;
        _ruleEngineProcessingService = ruleEngineProcessingService;
        _leadLastCacheRepository = leadLastCacheRepository;
    }

    public async Task Process(
        long clientId,
        IReadOnlyCollection<TrackedLead> leads,
        CancellationToken ct = default)
    {
        var rules = await _ruleRepository.GetRulesByType(clientId, RuleGroupTypes.LeadScoring, ct);

        _logger.LogInformation("Start {name} with rules {rules} for client Id {clientId}",
            nameof(ScoreProcessingService), string.Join(", ", rules.Select(r => r.Name)), clientId);

        await _ruleEngineProcessingService.Process(
            clientId,
            leads,
            rules,
            RuleGroupTypes.LeadScoring,
            ct);

        var leadsScores = leads.ToDictionary(r => r.LeadId, r => r.Score);
        await _leadLastCacheRepository.UpdateLeads(leadsScores);
    }
}
