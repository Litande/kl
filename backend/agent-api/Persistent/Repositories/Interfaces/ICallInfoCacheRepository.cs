using Plat4Me.DialAgentApi.Persistent.Entities.Cache;

namespace Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

public interface ICallInfoCacheRepository
{
    Task<CallInfoCache?> GetCallInfo(string sessionId);
}
