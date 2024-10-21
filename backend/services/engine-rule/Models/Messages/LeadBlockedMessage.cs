namespace Plat4Me.DialRuleEngine.Application.Models.Messages;

public record LeadBlockedMessage(long ClientId, long LeadId, long? QueueId = null);