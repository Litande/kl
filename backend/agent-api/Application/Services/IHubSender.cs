using KL.Agent.API.Application.Enums;
using KL.Agent.API.Application.Models.SignalR;

namespace KL.Agent.API.Application.Services;

public interface IHubSender
{
    Task SendCallInfo(string agentId, CallInfo message);
    Task SendCurrentStatus(string agentId, AgentStatusTypes status);
    Task SendAgentBlocked(long agentId);
    void DisconnectAgent(long agentId);
}