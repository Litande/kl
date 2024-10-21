using Plat4Me.DialClientApi.Persistent.Entities.Cache;

namespace Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

public interface ICallInfoCacheRepository
{
    Task<CallInfoCache?> GetCallInfo(string sessionId);
    Task<IDictionary<string, CallInfoCache>> GetCalls(long clientId, IEnumerable<long> agentIds);
    Task<CallInfoCache?> GetAgentByLead(long clientId, long leadId);
}
