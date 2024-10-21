using Plat4Me.DialAgentApi.Application.Enums;

namespace Plat4Me.DialAgentApi.Application.Models.Messages;

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
    public string Initiator => nameof(DialAgentApi);
}
