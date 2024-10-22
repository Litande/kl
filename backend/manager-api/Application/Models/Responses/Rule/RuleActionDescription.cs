using KL.Manager.API.Application.Models.Responses.Common;

namespace KL.Manager.API.Application.Models.Responses.Rule;

public record RuleActionDescription(
    string Name,
    string DisplayName,
    IEnumerable<LabelValue>? ActionOperation,
    RuleFieldDescription[]? Fields
);
