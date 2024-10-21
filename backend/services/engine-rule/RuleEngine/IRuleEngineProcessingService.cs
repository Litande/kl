using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.Enums;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine;

public interface IRuleEngineProcessingService
{
    Task Process(
        long clientId,
        IReadOnlyCollection<TrackedLead> leads,
        IReadOnlyCollection<RuleDto> rules,
        RuleGroupTypes ruleType,
        CancellationToken ct = default
    );
}
