using KL.Engine.Rule.Enums;

namespace KL.Engine.Rule.Models;

public record LeadStatusDto(
        long LeadId,
        long ClientId,
        LeadStatusTypes Status,
        DateTimeOffset? RemindOn,
        LeadSystemStatusTypes? SystemStatus
    );
