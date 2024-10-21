namespace Plat4Me.DialLeadCaller.Application.Models.Requests;

public record GetNextLeadRequest(
    long LeadQueueId,
    IEnumerable<long> WaitingAgentIds);
