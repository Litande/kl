namespace KL.Caller.Leads.Models.Responses;

public record GetNextLeadResponse(
    long LeadQueueId,
    long LeadId,
    string LeadPhone,
    long? AssignedAgentId,
    bool IsTest);
