namespace Plat4Me.DialClientApi.Application.Models.Responses.LeadQueues;

public record LeadQueuesResponse(IEnumerable<LeadQueueItemResponse> Items);