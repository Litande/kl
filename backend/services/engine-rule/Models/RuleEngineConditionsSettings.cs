using Plat4Me.DialRuleEngine.Application.Enums;

namespace Plat4Me.DialRuleEngine.Application.Models;

public record LeadFieldDescription(
    string Name,
    string DisplayName,
    LeadFieldTypes Type,
    LeadFieldTypes? SetElementType,
    LabelValue[]? SelectItems
);

public record RuleEngineConditionsSettings(LeadFieldDescription[]? LeadFields);
