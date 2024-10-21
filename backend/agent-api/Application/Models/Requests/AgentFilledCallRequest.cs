using Plat4Me.DialAgentApi.Application.Enums;

namespace Plat4Me.DialAgentApi.Application.Models.Requests;

public record AgentFilledCallRequest(
    string SessionId,
    LeadStatusTypes LeadStatus,
    DateTimeOffset? RemindOn,
    string? Comment = null
);
