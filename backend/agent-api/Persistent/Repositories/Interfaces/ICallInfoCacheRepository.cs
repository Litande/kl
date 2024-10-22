using KL.Agent.API.Persistent.Entities.Cache;

namespace KL.Agent.API.Persistent.Repositories.Interfaces;

public interface ICallInfoCacheRepository
{
    Task<CallInfoCache?> GetCallInfo(string sessionId);
}
