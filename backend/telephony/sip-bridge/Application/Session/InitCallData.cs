using KL.SIP.Bridge.Application.Enums;
using KL.SIP.Bridge.Application.Models;

namespace KL.SIP.Bridge.Application.Session;

public record InitCallData(
    long ClientId,
    CallType CallType,
    long? QueueId,
    long? LeadId,
    string LeadPhone,
    long AgentId,
    bool IsFixedAssigned,
    long? RingingTimeout,
    long? MaxCallDuration,
    string[]? IceServers,
    bool IsTest,
    SipProviderInfo SipProvider);
