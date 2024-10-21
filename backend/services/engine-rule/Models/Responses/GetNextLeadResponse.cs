namespace Plat4Me.DialRuleEngine.Application.Models.Responses;

public record GetNextLeadResponse(
    long LeadQueueId,
    long LeadId,
    string LeadPhone,
    long? AssignedAgentId,
    bool IsTest
    );
