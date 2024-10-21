namespace Plat4Me.DialLeadCaller.Application.Models.Responses;

public record GetNextLeadResponse(
    long LeadQueueId,
    long LeadId,
    string LeadPhone,
    long? AssignedAgentId,
    bool IsTest);
