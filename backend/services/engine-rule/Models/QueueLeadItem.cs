namespace Plat4Me.DialRuleEngine.Application.Models;

public record QueueLeadItem(
    long LeadId,
    long LeadScore,
    DateTimeOffset? AgentRemindOn = null,
    long? LastCallAgentId = null);