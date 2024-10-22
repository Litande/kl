using KL.Caller.Leads.Enums;

namespace KL.Caller.Leads.Models.Entities.Settings;

public record CallNaSettings(
    LeadStatusTypes DefaultNaStatus,
    string DefaultAgentComment
);
