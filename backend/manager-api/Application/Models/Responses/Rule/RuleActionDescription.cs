using Plat4Me.DialClientApi.Application.Models.Responses.Common;

namespace Plat4Me.DialClientApi.Application.Models.Responses.Rule;

public record RuleActionDescription(
    string Name,
    string DisplayName,
    IEnumerable<LabelValue>? ActionOperation,
    RuleFieldDescription[]? Fields
);
