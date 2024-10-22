using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Models;

namespace KL.Engine.Rule.RuleEngine;

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
