using KL.Agent.API.Application.Enums;
using KL.Agent.API.Application.Models.SignalR;

namespace KL.Agent.API.SignalR;

public interface IBaseClient
{
    Task CurrentStatus(AgentStatusTypes status);
    Task CallInfo(CallInfo message);
    Task AgentBlocked();
}
