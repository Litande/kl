namespace KL.Manager.API.Application.Models.Responses.LeadQueues;

public record LeadQueueItemResponse(
    long LeadQueueId,
    string Name);