namespace KL.Engine.Rule.Models;

public record QueueLeadItem(
    long LeadId,
    long LeadScore,
    DateTimeOffset? AgentRemindOn = null,
    long? LastCallAgentId = null);