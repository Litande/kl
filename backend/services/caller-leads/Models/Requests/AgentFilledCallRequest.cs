using Plat4Me.DialLeadCaller.Application.Enums;

namespace Plat4Me.DialLeadCaller.Application.Models.Requests;

public record AgentFilledCallRequest(
    string SessionId,
    LeadStatusTypes LeadStatus,
    DateTimeOffset? RemindOn,
    string? Comment = null
);
