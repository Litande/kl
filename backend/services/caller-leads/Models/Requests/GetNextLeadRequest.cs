namespace KL.Caller.Leads.Models.Requests;

public record GetNextLeadRequest(
    long LeadQueueId,
    IEnumerable<long> WaitingAgentIds);
