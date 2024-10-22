namespace KL.Engine.Rule.Models.Responses;

public record GetNextLeadResponse(
    long LeadQueueId,
    long LeadId,
    string LeadPhone,
    long? AssignedAgentId,
    bool IsTest
    );
