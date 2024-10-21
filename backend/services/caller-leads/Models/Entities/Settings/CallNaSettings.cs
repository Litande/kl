using Plat4Me.DialLeadCaller.Application.Enums;

namespace Plat4Me.DialLeadCaller.Application.Models.Entities.Settings;

public record CallNaSettings(
    LeadStatusTypes DefaultNaStatus,
    string DefaultAgentComment
);
