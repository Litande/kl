using KL.Agent.API.Application.Enums;
using KL.Agent.API.Application.Models.Messages;

namespace KL.Agent.API.Application.Services;

public interface IAgentStateService 
{
    Task AgentConnected(long agentId, long clientId, CancellationToken ct = default);
    Task AgentDisconnected(long agentId, long clientId,  CancellationToken ct = default);
    Task<AgentState> GetAgentCurrentState(long agentId, long clientId, CancellationToken ct = default);
    Task<bool> CanStartManualCall(long agentId, long clientId, CancellationToken ct = default);
    Task ChangeAgentStatus(long agentId, long clientId, AgentInternalStatusTypes status, bool forcePublish = false, CancellationToken ct = default);
    Task ChangeAgentStatus(long agentId, long clientId, AgentStatusTypes status, bool forcePublish  = false, CancellationToken ct = default);

    Task Handle(LeadFeedbackFilledMessage message, CancellationToken ct = default);
    Task Handle(InviteAgentMessage message, CancellationToken ct = default);
    Task Handle(CallInitiatedMessage message, CancellationToken ct = default);
    Task Handle(CallFinishedMessage message, CancellationToken ct = default);
    Task Handle(CalleeAnsweredMessage message, CancellationToken ct = default);
    Task Handle(DroppedAgentMessage message, CancellationToken ct = default);
    Task Handle(AgentReplacedMessage message, CancellationToken ct = default);
}
