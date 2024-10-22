using KL.Caller.Leads.Models;

namespace KL.Caller.Leads.Repositories;

public interface ICallInfoCacheRepository
{
    Task<CallInfoCache?> GetCallInfo(string sessionId);
    Task UpdateCallInfo(CallInfoCache cache);
    Task IncreaseCallAgain(string sessionId);
    Task RemoveCallInfo(string sessionId);
    string LockPrefix { get; }
}
