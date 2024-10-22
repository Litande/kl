namespace KL.Manager.API.Application.Models.Messages.Lead;

public record LeadBlockedMessage(long ClientId, long LeadId, long? QueueId = null);