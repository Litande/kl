using Plat4Me.DialAgentApi.Application.Enums;

namespace Plat4Me.DialAgentApi.Application.Services;

public interface IAgentTimeoutService : IDisposable
{
    bool TryCancelTimeout(AgentTimeoutTypes type, string key);
    bool SetTimeout(AgentTimeoutTypes type, string key, long dueTime, Action onTimeout);
}
