namespace Plat4Me.DialClientApi.Application.Models.Messages.Lead;

public record LeadBlockedMessage(long ClientId, long LeadId, long? QueueId = null);