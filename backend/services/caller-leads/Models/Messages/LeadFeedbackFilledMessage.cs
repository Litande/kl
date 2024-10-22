using KL.Caller.Leads.Enums;

namespace KL.Caller.Leads.Models.Messages;

public record LeadFeedbackFilledMessage(
    long ClientId,
    long AgentId,
    long? QueueId,
    string SessionId,
    long LeadId,
    LeadStatusTypes LeadStatus,
    DateTimeOffset? RemindOn,
    string Initiator,
    string? AgentComment = null);
