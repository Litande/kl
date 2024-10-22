using KL.Engine.Rule.Models;

namespace KL.Engine.Rule.RuleEngine.Contracts;

public record RuleConditionDescription(
    string Name,
    string DisplayName,
    string Category,
    IEnumerable<LabelValue>? ComparisonOperation,
    RuleFieldDescription[]? Fields
);
