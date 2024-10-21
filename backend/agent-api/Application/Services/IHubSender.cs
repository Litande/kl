using Plat4Me.DialAgentApi.Application.Enums;
using Plat4Me.DialAgentApi.Application.Models.SignalR;

namespace Plat4Me.DialAgentApi.Application.Services;

public interface IHubSender
{
    Task SendCallInfo(string agentId, CallInfo message);
    Task SendCurrentStatus(string agentId, AgentStatusTypes status);
    Task SendAgentBlocked(long agentId);
    void DisconnectAgent(long agentId);
}