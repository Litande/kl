using KL.Agent.API.Application.Enums;

namespace KL.Agent.API.Application.Models.Messages;

public record CallInitiatedMessage(
    long ClientId,
    string BridgeId,
    string SessionId,
    CallType CallType,
    long AgentId,
    long? QueueId,
    long? LeadId,
    string LeadPhone
    );
