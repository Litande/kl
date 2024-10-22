using KL.Caller.Leads.Enums;

namespace KL.Caller.Leads.Models.Messages;

public record LeadFeedbackCallFailedMessage(
    long ClientId,
    long AgentId,
    long? QueueId,
    string SessionId,
    long LeadId,
    LeadStatusTypes LeadStatus,
    DateTimeOffset? RemindOn = null)
{
    protected string Initiator => nameof(KL.Caller);
}
