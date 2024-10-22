using KL.Caller.Leads.Enums;

namespace KL.Caller.Leads.Models.Requests;

public record AgentFilledCallRequest(
    string SessionId,
    LeadStatusTypes LeadStatus,
    DateTimeOffset? RemindOn,
    string? Comment = null
);
