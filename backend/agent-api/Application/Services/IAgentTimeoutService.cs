using KL.Agent.API.Application.Enums;

namespace KL.Agent.API.Application.Services;

public interface IAgentTimeoutService : IDisposable
{
    bool TryCancelTimeout(AgentTimeoutTypes type, string key);
    bool SetTimeout(AgentTimeoutTypes type, string key, long dueTime, Action onTimeout);
}
