using Plat4Me.DialAgentApi.Application.Enums;
using Plat4Me.DialAgentApi.Application.Models.SignalR;

namespace Plat4Me.DialAgentApi.SignalR;

public interface IBaseClient
{
    Task CurrentStatus(AgentStatusTypes status);
    Task CallInfo(CallInfo message);
    Task AgentBlocked();
}
