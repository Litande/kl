using Plat4Me.DialSipBridge.Application.Session;

namespace Plat4Me.DialSipBridge.Application.Services;

public interface ICallService
{
    Task<string?> CreateSession(string sessionId, InitCallData callData);
    ICallSession? GetSession(string sessionId);
    void CloseSession(string sessionId);
    long SessionCount();
}
