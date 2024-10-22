using KL.SIP.Bridge.Application.Enums;

namespace KL.SIP.Bridge.Application.Models.Messages;

public record CallToLeadMessage(
    long ClientId,
    string BridgeId,
    string SessionId,
    CallType CallType,
    long? QueueId,
    long? LeadId,
    bool IsFixedAssigned,
    string Phone,
    long AgentId,
    long? RingingTimeout,
    long? MaxCallDuration,
    string[]? IceServers,
    bool IsTest,
    string Initiator,
    SipProviderInfo SipProvider);
