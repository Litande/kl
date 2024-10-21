namespace Plat4Me.DialClientApi.Application.Models.Responses.Common;

public record RuleFieldDescription(
    int FieldId,
    string Type,
    string Dimension,
    IEnumerable<LabelValue>? Values,
    bool IsRequired
);
