namespace KL.Manager.API.Application.Models.Responses.LeadQueues;

public record LeadQueuesResponse(IEnumerable<LeadQueueItemResponse> Items);