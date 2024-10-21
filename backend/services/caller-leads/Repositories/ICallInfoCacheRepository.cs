using Plat4Me.DialLeadCaller.Application.Models;

namespace Plat4Me.DialLeadCaller.Application.Repositories;

public interface ICallInfoCacheRepository
{
    Task<CallInfoCache?> GetCallInfo(string sessionId);
    Task UpdateCallInfo(CallInfoCache cache);
    Task IncreaseCallAgain(string sessionId);
    Task RemoveCallInfo(string sessionId);
    string LockPrefix { get; }
}
