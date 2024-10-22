using KL.Engine.Rule.Enums;

namespace KL.Engine.Rule.Models;

public record LeadFieldDescription(
    string Name,
    string DisplayName,
    LeadFieldTypes Type,
    LeadFieldTypes? SetElementType,
    LabelValue[]? SelectItems
);

public record RuleEngineConditionsSettings(LeadFieldDescription[]? LeadFields);
