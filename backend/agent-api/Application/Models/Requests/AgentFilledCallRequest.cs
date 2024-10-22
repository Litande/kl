using KL.Agent.API.Application.Enums;

namespace KL.Agent.API.Application.Models.Requests;

public record AgentFilledCallRequest(
    string SessionId,
    LeadStatusTypes LeadStatus,
    DateTimeOffset? RemindOn,
    string? Comment = null
);
