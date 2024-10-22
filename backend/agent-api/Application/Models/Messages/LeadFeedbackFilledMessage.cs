using KL.Agent.API.Application.Enums;

namespace KL.Agent.API.Application.Models.Messages;

public record LeadFeedbackFilledMessage(
    long ClientId,
    long AgentId,
    long? QueueId,
    string SessionId,
    long LeadId,
    LeadStatusTypes LeadStatus,
    DateTimeOffset? RemindOn,
    string? AgentComment = null)
{
    public string Initiator => nameof(KL.Agent.API);
}
