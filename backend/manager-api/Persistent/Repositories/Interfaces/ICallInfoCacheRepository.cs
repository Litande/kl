using KL.Manager.API.Persistent.Entities.Cache;

namespace KL.Manager.API.Persistent.Repositories.Interfaces;

public interface ICallInfoCacheRepository
{
    Task<CallInfoCache?> GetCallInfo(string sessionId);
    Task<IDictionary<string, CallInfoCache>> GetCalls(long clientId, IEnumerable<long> agentIds);
    Task<CallInfoCache?> GetAgentByLead(long clientId, long leadId);
}
