namespace KL.Engine.Rule.Models.Messages;

public record LeadBlockedMessage(long ClientId, long LeadId, long? QueueId = null);