using Plat4Me.DialSipBridge.Application.Enums;
using Plat4Me.DialSipBridge.Application.Models;

namespace Plat4Me.DialSipBridge.Application.Session;

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
