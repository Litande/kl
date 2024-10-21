using Plat4Me.DialLeadCaller.Application.Enums;

namespace Plat4Me.DialLeadCaller.Application.Models.Messages;

public record AgentNotAnsweredMessage(
    long ClientId,
    string BridgeId,
    string SessionId,
    CallType CallType,
    long AgentId,
    long QueueId,
    long? LeadId,
    bool IsFixedAssigned,
    string LeadPhone,
    string Initiator,
    long SipProviderId);
