using System.Collections.Concurrent;
using KL.Agent.API.Application.Enums;

namespace KL.Agent.API.Application.Services;

public class AgentTimeoutService : IAgentTimeoutService
{
    private Dictionary<AgentTimeoutTypes, ConcurrentDictionary<string, IDisposable>> _timeouts = new();

    protected bool _disposed = false;

    public AgentTimeoutService()
    {
        _timeouts.Add(AgentTimeoutTypes.ConnectionTimeout, new ConcurrentDictionary<string, IDisposable>());
        _timeouts.Add(AgentTimeoutTypes.FeedbackTimeout, new ConcurrentDictionary<string, IDisposable>());
    }

    public bool TryCancelTimeout(AgentTimeoutTypes type, string key)
    {
        if (_timeouts[type].Remove(key, out var timeout))
        {
            timeout.Dispose();
            return true;
        }
        return false;
    }

    public bool SetTimeout(AgentTimeoutTypes type, string key, long dueTime, Action onTimeout)
    {
        if (!_timeouts[type].TryGetValue(key, out var _))
        {
            var timeout = new Timer((object? state) =>
            {
                if (_timeouts[type].Remove(key, out var timeout))
                {
                    onTimeout.Invoke();
                    timeout.Dispose();
                }
            }, null, dueTime, Timeout.Infinite);
            return _timeouts[type].TryAdd(key, timeout);
        }
        return false;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        ClearTimeouts(_timeouts[AgentTimeoutTypes.ConnectionTimeout]);
        ClearTimeouts(_timeouts[AgentTimeoutTypes.FeedbackTimeout]);
    }

    private void ClearTimeouts(ConcurrentDictionary<string, IDisposable> timeouts)
    {
        foreach (var (id, timeout) in timeouts)
        {
            timeout.Dispose();
        }
        timeouts.Clear();
    }
}
