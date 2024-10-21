using Plat4Me.DialAgentApi.Persistent.Entities.Cache;

namespace Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

public interface ILeadCacheRepository
{
    Task<LeadTrackingCache?> GetLead(long leadId);
    Task<Dictionary<long, LeadTrackingCache>> GetLeads(IEnumerable<long> leadIds);
}
