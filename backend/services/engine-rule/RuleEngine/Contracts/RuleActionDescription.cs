using Plat4Me.DialRuleEngine.Application.Models;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;

public record RuleActionDescription(
    string Name,
    string DisplayName,
    IEnumerable<LabelValue>? ActionOperation,
    RuleFieldDescription[]? Fields
);
