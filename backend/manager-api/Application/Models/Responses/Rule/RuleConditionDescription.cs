using Plat4Me.DialClientApi.Application.Models.Responses.Common;

namespace Plat4Me.DialClientApi.Application.Models.Responses.Rule;

public record RuleConditionDescription(
    string Name, //ConditionRules
    string DisplayName,
    IEnumerable<LabelValue>? ComparisonOperation, // ComparisonOperation
    RuleFieldDescription[]? Fields
);
