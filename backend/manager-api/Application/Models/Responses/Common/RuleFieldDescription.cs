namespace KL.Manager.API.Application.Models.Responses.Common;

public record RuleFieldDescription(
    int FieldId,
    string Type,
    string Dimension,
    IEnumerable<LabelValue>? Values,
    bool IsRequired
);
