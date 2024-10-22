using KL.Manager.API.Application.Models.Responses.Common;

namespace KL.Manager.API.Application.Models.Responses.Rule;

public record RuleConditionDescription(
    string Name, //ConditionRules
    string DisplayName,
    IEnumerable<LabelValue>? ComparisonOperation, // ComparisonOperation
    RuleFieldDescription[]? Fields
);
