using Plat4Me.DialRuleEngine.Application.Enums;

namespace Plat4Me.DialRuleEngine.Application.Models.Messages;

public record LeadFeedbackFilledMessage(
    long ClientId,
    long AgentId,
    long? QueueId,
    long LeadId,
    LeadStatusTypes LeadStatus,
    DateTimeOffset? RemindOn,
    string Initiator,
    string? AgentComment = null);
