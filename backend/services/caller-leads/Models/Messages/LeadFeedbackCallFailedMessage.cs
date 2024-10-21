using Plat4Me.DialLeadCaller.Application.Enums;

namespace Plat4Me.DialLeadCaller.Application.Models.Messages;

public record LeadFeedbackCallFailedMessage(
    long ClientId,
    long AgentId,
    long? QueueId,
    string SessionId,
    long LeadId,
    LeadStatusTypes LeadStatus,
    DateTimeOffset? RemindOn = null)
{
    protected string Initiator => nameof(DialLeadCaller);
}
