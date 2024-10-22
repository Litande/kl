using KL.SIP.Bridge.Application.Enums;

namespace KL.SIP.Bridge.Application.Models.Messages;

public record AgentNotAnsweredMessage(
    long ClientId,
    string BridgeId,
    string SessionId,
    CallType CallType,
    long? QueueId,
    long AgentId,
    long? LeadId,
    bool IsFixedAssigned,
    string LeadPhone,
    long SipProviderId);
