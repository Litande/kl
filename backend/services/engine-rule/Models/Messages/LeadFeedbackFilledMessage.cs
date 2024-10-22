using KL.Engine.Rule.Enums;

namespace KL.Engine.Rule.Models.Messages;

public record LeadFeedbackFilledMessage(
    long ClientId,
    long AgentId,
    long? QueueId,
    long LeadId,
    LeadStatusTypes LeadStatus,
    DateTimeOffset? RemindOn,
    string Initiator,
    string? AgentComment = null);
