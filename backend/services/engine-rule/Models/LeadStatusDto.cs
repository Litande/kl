using Plat4Me.DialRuleEngine.Application.Enums;

namespace Plat4Me.DialRuleEngine.Application.Models;

public record LeadStatusDto(
        long LeadId,
        long ClientId,
        LeadStatusTypes Status,
        DateTimeOffset? RemindOn,
        LeadSystemStatusTypes? SystemStatus
    );
