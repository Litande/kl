using KL.SIP.Bridge.Application.Session;

namespace KL.SIP.Bridge.Application.Services;

public interface ICallService
{
    Task<string?> CreateSession(string sessionId, InitCallData callData);
    ICallSession? GetSession(string sessionId);
    void CloseSession(string sessionId);
    long SessionCount();
}
