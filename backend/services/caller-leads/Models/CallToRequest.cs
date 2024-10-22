using KL.Caller.Leads.Enums;

namespace KL.Caller.Leads.Models;

public record CallToRequest(
    long ClintId,
    string BridgeId,
    CallType CallType,
    long? LeadQueueId,
    long? LeadId,
    bool IsFixedAssigned,
    string Phone,
    long AgentId,
    long? RingingTimeout,
    long? MaxCallDuration,
    string[]? IceServers,
    bool IsTest,
    SipProviderInfo SipProvider
);
