using KL.Engine.Rule.Models;

namespace KL.Engine.Rule.RuleEngine.Contracts;

public record RuleActionDescription(
    string Name,
    string DisplayName,
    IEnumerable<LabelValue>? ActionOperation,
    RuleFieldDescription[]? Fields
);
