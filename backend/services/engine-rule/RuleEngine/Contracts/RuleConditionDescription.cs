using Plat4Me.DialRuleEngine.Application.Models;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;

public record RuleConditionDescription(
    string Name,
    string DisplayName,
    string Category,
    IEnumerable<LabelValue>? ComparisonOperation,
    RuleFieldDescription[]? Fields
);
